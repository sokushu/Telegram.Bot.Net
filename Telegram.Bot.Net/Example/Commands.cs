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
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Framework;
using Telegram.Bot.Framework.TelegramAttributes;
using Telegram.Bot.Framework.TelegramException;
using Telegram.Bot.Framework.TelegramMessage;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Net.Example
{
    public class Commands : TelegramController
    {
        private readonly IServiceProvider serviceProvider;
        public Commands(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        [Command("Test")]
        public async Task Test()
        {
            await SendTextMessage("你好");
            await SendTextMessage("打开百度", new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData("百度一下", "CallBack_baidu") });
        }
    }
}
