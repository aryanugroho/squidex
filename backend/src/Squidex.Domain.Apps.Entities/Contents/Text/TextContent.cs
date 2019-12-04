﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Text;
using Squidex.Domain.Apps.Core.Contents;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Json.Objects;

namespace Squidex.Domain.Apps.Entities.Contents.Text
{
    public sealed class TextContent
    {
        public Dictionary<string, string> Texts { get; } = new Dictionary<string, string>();

        public string this[string key]
        {
            get
            {
                return Texts.GetOrDefault(key) ?? string.Empty;
            }
            set
            {
                Texts[key] = value;
            }
        }

        public TextContent()
        {
        }

        public TextContent(NamedContentData data)
        {
            if (data == null)
            {
                return;
            }

            var languages = new Dictionary<string, StringBuilder>();

            void AppendText(string language, string text)
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    var sb = languages.GetOrAddNew(language);

                    if (sb.Length > 0)
                    {
                        sb.Append(" ");
                    }

                    sb.Append(text);
                }
            }

            foreach (var field in data)
            {
                if (field.Value != null)
                {
                    foreach (var fieldValue in field.Value)
                    {
                        var appendText = new Action<string>(text => AppendText(fieldValue.Key, text));

                        AppendJsonText(fieldValue.Value, appendText);
                    }
                }
            }

            foreach (var kvp in languages)
            {
                Texts[kvp.Key] = kvp.Value.ToString();
            }
        }

        private static void AppendJsonText(IJsonValue value, Action<string> appendText)
        {
            if (value.Type == JsonValueType.String)
            {
                appendText(value.ToString());
            }
            else if (value is JsonArray array)
            {
                foreach (var item in array)
                {
                    AppendJsonText(item, appendText);
                }
            }
            else if (value is JsonObject obj)
            {
                foreach (var item in obj.Values)
                {
                    AppendJsonText(item, appendText);
                }
            }
        }
    }
}