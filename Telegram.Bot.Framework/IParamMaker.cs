﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Telegram.Bot.Framework
{
    public interface IParamMaker
    {
        Task<object> GetParam(TelegramContext context, IServiceProvider serviceProvider);
    }
}
