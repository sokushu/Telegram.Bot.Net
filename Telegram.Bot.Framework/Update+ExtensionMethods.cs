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

using Telegram.Bot.Types;

namespace Telegram.Bot.Framework
{
    /// <summary>
    /// 一个扩展方法，用于扩展 <see cref="Update"/> 对象
    /// </summary>
    internal static class Update_ExtensionMethods
    {
        /// <summary>
        /// 从 <see cref="Update"/> 中获取 <see cref="ChatId"/> 对象
        /// </summary>
        /// <param name="update">传入的 <see cref="Update"/> 对象</param>
        /// <returns><see cref="ChatId"/> 对象</returns>
        public static ChatId GetChatID(this Update update)
        {
            switch (update.Type)
            {
                case Types.Enums.UpdateType.Unknown:
                    break;
                case Types.Enums.UpdateType.Message:
                    return update.Message.Chat.Id;
                case Types.Enums.UpdateType.InlineQuery:
                    break;
                case Types.Enums.UpdateType.ChosenInlineResult:
                    break;
                case Types.Enums.UpdateType.CallbackQuery:
                    return update.CallbackQuery.Message.Chat.Id;
                case Types.Enums.UpdateType.EditedMessage:
                    return update.EditedMessage.Chat.Id;
                case Types.Enums.UpdateType.ChannelPost:
                    return update.ChannelPost.Chat.Id;
                case Types.Enums.UpdateType.EditedChannelPost:
                    return update.EditedChannelPost.Chat.Id;
                case Types.Enums.UpdateType.ShippingQuery:
                    break;
                case Types.Enums.UpdateType.PreCheckoutQuery:
                    break;
                case Types.Enums.UpdateType.Poll:
                    break;
                case Types.Enums.UpdateType.PollAnswer:
                    break;
                case Types.Enums.UpdateType.MyChatMember:
                    return update.MyChatMember.Chat.Id;
                case Types.Enums.UpdateType.ChatMember:
                    return update.ChatMember.Chat.Id;
                case Types.Enums.UpdateType.ChatJoinRequest:
                    return update.ChatJoinRequest.Chat.Id;
            }
            return null;
        }

        /// <summary>
        /// 从 <see cref="Update"/> 中获取用户输入的指令信息
        /// </summary>
        /// <param name="update">传入的 <see cref="Update"/> 对象</param>
        /// <returns>指令字符串</returns>
        public static string GetCommand(this Update update)
        {
            if (update.Message == null || string.IsNullOrEmpty(update.Message.Text))
                return null;
            string command = update.Message.Text;
            return command.StartsWith("/") ? command : null;
        }

        /// <summary>
        /// 将一个 <see cref="Update"/> 中的内容，复制到另一个 <see cref="Update"/> 中
        /// </summary>
        /// <param name="souce">想要进行复制的对象</param>
        /// <param name="target">复制的目标对象</param>
        internal static void CopyTo(this Update souce, Update target)
        {
            souce.Poll = target.Poll;
            souce.ChatMember = target.ChatMember;
            souce.ChatJoinRequest = target.ChatJoinRequest;
            souce.CallbackQuery = target.CallbackQuery;
            souce.ChannelPost = target.ChannelPost;
            souce.ChosenInlineResult = target.ChosenInlineResult;
            souce.EditedChannelPost = target.EditedChannelPost;
            souce.EditedMessage = target.EditedMessage;
            souce.Id = target.Id;
            souce.InlineQuery = target.InlineQuery;
            souce.Message = target.Message;
            souce.MyChatMember = target.MyChatMember;
            souce.PollAnswer = target.PollAnswer;
            souce.PreCheckoutQuery = target.PreCheckoutQuery;
            souce.ShippingQuery = target.ShippingQuery;
        }
    }
}
