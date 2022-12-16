﻿//  <Telegram.Bot.Framework>
//  Copyright (C) <2022>  <Azumo-Lab> see <https://github.com/Azumo-Lab/Telegram.Bot.Framework/>
//
//  This file is part of <Telegram.Bot.Framework>: you can redistribute it and/or modify
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Telegram.Bot.Framework.InternalFramework.Models;
using Telegram.Bot.Framework.InternalFramework.Abstract;

namespace Telegram.Bot.Framework
{
    public abstract class TelegramController
    {
        public TelegramContext Context { get; internal set; }

        /// <summary>
        /// 执行调用
        /// </summary>
        internal async Task Invoke(TelegramContext context, string CommandName)
        {
            Context = context;

            IServiceProvider userService = context.UserScope;

            IDelegateManager delegateManger = userService.GetService<IDelegateManager>();
            IParamManager paramManger = userService.GetService<IParamManager>();

            Delegate action = delegateManger.CreateDelegate(CommandName, this);
            object[] Params = paramManger.GetParam();

            Action commandAction = null;

            if (Params.IsEmpty())
                commandAction = () => action.DynamicInvoke();
            else
                commandAction = () => action.DynamicInvoke(Params);

            await Task.Run(commandAction);
        }

        /// <summary>
        /// 执行调用
        /// </summary>
        internal async Task Invoke(TelegramContext context, CommandInfos CommandName)
        {
            Context = context;

            IDelegateManager delegateManger = context.UserScope.GetService<IDelegateManager>();

            Delegate action = delegateManger.CreateDelegate(CommandName, this);

            Action commandAction = null;

            commandAction = () => action.DynamicInvoke();

            await Task.Run(commandAction);
        }
    }
}
