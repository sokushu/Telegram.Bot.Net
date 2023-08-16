﻿//  <Telegram.Bot.Framework>
//  Copyright (C) <2022 - 2023>  <Azumo-Lab> see <https://github.com/Azumo-Lab/Telegram.Bot.Framework/>
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

namespace Telegram.Bot.Framework.Abstracts.Bot
{
    /// <summary>
    /// 对Bot设定的一些信息
    /// </summary>
    public interface IBotInfo
    {
        /// <summary>
        /// Bot的名称
        /// </summary>
        public string BotName { get; internal set; }

        /// <summary>
        /// Bot的ID
        /// </summary>
        public long BotID { get; internal set; }

        /// <summary>
        /// Bot的启动时间
        /// </summary>
        public DateTime BotStartTime { get; internal set; }

        /// <summary>
        /// Bot的运行时长
        /// </summary>
        /// <returns></returns>
        public TimeSpan RunTimes();
    }
}
