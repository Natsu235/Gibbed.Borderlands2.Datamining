﻿/* Copyright (c) 2019 Rick (rick 'at' gibbed 'dot' us)
 *
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 *
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 *
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Gibbed.Unreflect.Core;
using Dataminer = Borderlands2Datamining.Dataminer;

namespace DumpCurrencies
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            new Dataminer().Run(args, Go);
        }

        private static void Go(Engine engine)
        {
            var currencyListDefinitionClass = engine.GetClass("WillowGame.CurrencyListDefinition");
            if (currencyListDefinitionClass == null)
            {
                throw new InvalidOperationException();
            }

            var currencyListDefinitions = engine.Objects
                    .Where(o => o.IsA(currencyListDefinitionClass) &&
                                o.GetName().StartsWith("Default__") == false)
                    .OrderBy(o => o.GetPath());

            using (var writer = Dataminer.NewDump("Currencies.json"))
            {
                writer.WriteStartObject();
                foreach (dynamic currencyListDefinition in currencyListDefinitions)
                {
                    writer.WritePropertyName(currencyListDefinition.GetPath());
                    writer.WriteStartObject();

                    writer.WritePropertyName("currencies");
                    writer.WriteStartArray();
                    foreach (var currency in currencyListDefinition.Currencies)
                    {
                        writer.WriteStartObject();

                        writer.WritePropertyName("type");
                        writer.WriteValue(((CurrencyType)currency.Type).ToString());

                        writer.WritePropertyName("widget_frame");
                        writer.WriteValue(currency.WidgetFrame);

                        writer.WritePropertyName("widget_clip");
                        writer.WriteValue(currency.WidgetClip.GetPath());

                        writer.WriteEndObject();
                    }

                    writer.WriteEndArray();
                    writer.WriteEndObject();
                }

                writer.WriteEndObject();
                writer.Flush();
            }
        }
    }
}
