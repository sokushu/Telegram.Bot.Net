﻿//  <Telegram.Bot.Framework>
//  Copyright (C) <2022 - 2024>  <Azumo-Lab> see <https://github.com/Azumo-Lab/Telegram.Bot.Framework/>
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azumo.PipelineMiddleware.Pipelines;
internal class DefaultPipelineController<TInput> : IPipelineController<TInput>
{
    private MiddlewareDelegate<TInput>? middleware;
    MiddlewareDelegate<TInput>? IPipelineController<TInput>.NextHandle
    {
        get => middleware;
        set => middleware = value;
    }

    private readonly Dictionary<object, IPipeline<TInput>> __PipelineDic = [];

    public void AddPipeline(IPipeline<TInput> pipeline, object name)
    {
        if (__PipelineDic.TryAdd(name, pipeline))
            __PipelineDic[name] = pipeline;
    }
    public Task Execute(object name, TInput input) => 
        __PipelineDic.TryGetValue(name, out var pipeline) ? pipeline.Invoke(input) : Task.CompletedTask;

    public Task Next(TInput input) => middleware?.Invoke(input, this) ?? Task.CompletedTask;
    public void RemovePipeline(object name) => 
        __PipelineDic.Remove(name);
    public Task Stop(TInput input) => Task.CompletedTask;
}
