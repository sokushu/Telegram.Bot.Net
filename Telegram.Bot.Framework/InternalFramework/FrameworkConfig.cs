﻿//  < Telegram.Bot.Framework >
//  Copyright (C) <2022>  <Sokushu> see <https://github.com/sokushu/Telegram.Bot.Net/>
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <https://www.gnu.org/licenses/>.

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Telegram.Bot.Framework.InternalFramework.ControllerManger;
using Telegram.Bot.Framework.InternalFramework.InterFaces;
using Telegram.Bot.Framework.InternalFramework.ParameterManger;
using Telegram.Bot.Framework.TelegramAttributes;
using Telegram.Bot.Framework.TelegramException;
using Telegram.Bot.Polling;

namespace Telegram.Bot.Framework.InternalFramework
{
    internal class FrameworkConfig : IConfig
    {
        private readonly List<IConfig> setUps;
        private readonly ITelegramBotClient botClient;

        public FrameworkConfig(List<IConfig> setUps, bool UseBotName, ITelegramBotClient telegramBot)
        {
            this.setUps = setUps;
            botClient = telegramBot;
        }

        /// <summary>
        /// 框架相关的一些设置
        /// </summary>
        /// <param name="telegramServices"></param>
        public void Config(IServiceCollection telegramServices)
        {
            telegramServices.AddScoped<IParamMessage, StringParamMessage>();
            telegramServices.AddTransient<ITelegramRouteUserController, TelegramRouteUserController>();
            telegramServices.AddTransient<ITelegramUserScope, TelegramUserScope>();
            telegramServices.AddSingleton(new CancellationTokenSource());
            telegramServices.AddSingleton<IUpdateHandler, UpdateHandler>();
            telegramServices.AddSingleton(botClient);
            telegramServices.AddScoped(x =>
            {
                return new TelegramContext();
            });

            telegramServices.AddControllers();

            setUps.ForEach(x => x.Config(telegramServices));
        }
    }

    internal static class ServiceCollectionEx
    {
        /// <summary>
        /// 添加控制器
        /// </summary>
        /// <param name="services"></param>
        public static void AddControllers(this IServiceCollection services)
        {
            List<Type> AllTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).ToList();

            Type basetype = typeof(TelegramController);
            List<Type> TelegramControllerTypes = AllTypes.Where(x => basetype.IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface).ToList();
            basetype = typeof(IParamMaker);
            List<Type> IParamMakerTypes = AllTypes.Where(x => basetype.IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface).ToList();
            IParamMakerTypes.ForEach(x => services.AddScoped(x));

            Dictionary<string, Type> Command_ControllerMap = new Dictionary<string, Type>();
            Dictionary<string, MethodInfo> Command_MethodMap = new Dictionary<string, MethodInfo>();
            Dictionary<string, List<(Type ParamType, string ParamMessage)>> Command_ParamInfos = new Dictionary<string, List<(Type ParamType, string ParamMessage)>>();

            foreach (Type item in TelegramControllerTypes)
            {
                //把控制器添加进入Scoped
                services.AddScoped(item);

                var methods = item.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                foreach (var method in methods)
                {
                    CommandAttribute attr = (CommandAttribute)Attribute.GetCustomAttribute(method, typeof(CommandAttribute));
                    if (attr == null)
                        continue;
                    if (Command_ControllerMap.ContainsKey(attr.CommandName))
                        throw new RepeatedCommandException(attr.CommandName);
                    //指令名称和控制器的关系
                    Command_ControllerMap.Add(attr.CommandName, item);
                    //指令名称和方法的关系
                    Command_MethodMap.Add(attr.CommandName, method);
                    Command_ParamInfos.Add(attr.CommandName, new List<(Type ParamType, string ParamMessage)>());

                    //处理参数相关的一些操作
                    var methodParams = method.GetParameters().ToList();

                    foreach (var para in methodParams)
                    {
                        var attrParam = (ParamAttribute)Attribute.GetCustomAttribute(para, typeof(ParamAttribute));
                        if (attrParam == null)
                            continue;

                        string message;
                        if (attrParam.UseCustom)
                        {
                            message = attrParam.CustomInfos;
                        }
                        else
                        {
                            message = $"请输入【{attrParam.CustomInfos}】的值";
                        }
                        
                        Type IParamMakerType = IParamMakerTypes.Where(x => 
                        {
                            return Attribute.IsDefined(x, typeof(ParamMakerAttribute));
                        }).FirstOrDefault();
                        Command_ParamInfos[attr.CommandName].Add((IParamMakerType, message));
                    }
                }
            }

            //设置方法与指令的对应信息
            ControllersManger.SetDic(Command_ControllerMap);
            DelegateManger.SetDic(Command_MethodMap);
            ParamManger.SetDic(Command_ParamInfos);

            //添加进入services
            services.AddScoped<IControllersManger, ControllersManger>();
            services.AddScoped<IDelegateManger, DelegateManger>();
            services.AddScoped<IParamManger, ParamManger>();
        }

        /// <summary>
        /// 添加认证
        /// </summary>
        /// <param name="services"></param>
        public static void AddAuthentication(this IServiceCollection services)
        {

        }
    }
}