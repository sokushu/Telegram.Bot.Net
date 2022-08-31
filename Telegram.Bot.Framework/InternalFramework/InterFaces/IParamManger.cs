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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Telegram.Bot.Framework.InternalFramework.InterFaces
{
    /// <summary>
    /// 帮助创建参数
    /// </summary>
    internal interface IParamManger
    {
        /// <summary>
        /// 是否处于读取参数的模式
        /// </summary>
        /// <returns></returns>
        bool IsReadParam();

        /// <summary>
        /// 取消读取参数
        /// </summary>
        void Cancel();

        /// <summary>
        /// 设置指令
        /// </summary>
        void SetCommand();

        /// <summary>
        /// 设置指令
        /// </summary>
        Task<bool> StartReadParam();

        /// <summary>
        /// 获取读取过后的参数
        /// </summary>
        /// <returns></returns>
        object[] GetParam();

        /// <summary>
        /// 获取Command的名称
        /// </summary>
        /// <returns></returns>
        string GetCommand();
    }
}