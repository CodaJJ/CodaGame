// Copyright (c) 2026 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace CodaGame
{
    /// <summary>
    /// A string template parser that supports parameter substitution and custom functions.
    /// </summary>
    /// <remarks>
    /// <b>Syntax:</b>
    /// <list type="bullet">
    ///   <item><c>{0}, {1}, ...</c> - Parameter placeholders, replaced by args passed to Parse()</item>
    ///   <item><c>{funcName(arg1, arg2)}</c> - Custom function call</item>
    ///   <item><c>{{</c> and <c>}}</c> - Escaped braces, output as literal { and }</item>
    /// </list>
    ///
    /// <b>Example:</b>
    /// <code>
    /// var parser = new StringTemplateParser();
    /// parser.RegisterFunction("upper", args => args[0].ToUpper());
    ///
    /// parser.Parse("Hello {0}!", "World");              // "Hello World!"
    /// parser.Parse("{upper({0})}!", "world");           // "WORLD!"
    /// parser.Parse("Use {{braces}}");                   // "Use {braces}"
    /// </code>
    /// </remarks>
    public class StringTemplateParser
    {
        [NotNull] private readonly Dictionary<string, NotNullFunc<string[], string>> _m_functions;
        
        private const char LeftBracesPlaceholder = '\uE000'; // 私用区字符占位
        private const char RightBracesPlaceholder = '\uE001'; // 私用区字符占位
        

        public StringTemplateParser()
        {
            _m_functions = new Dictionary<string, NotNullFunc<string[], string>>();
        }
        

        /// <summary>
        /// Registers a custom function that can be called in templates.
        /// </summary>
        public void RegisterFunction(string _name, NotNullFunc<string[], string> _func)
        {
            if (string.IsNullOrEmpty(_name) || _func == null)
            {
                Console.LogWarning(SystemNames.TemplateParser, "Function name and implementation cannot be null or empty");
                return;
            }
            
            if (!_m_functions.TryAdd(_name, _func))
                Console.LogWarning(SystemNames.TemplateParser, $"Function '{_name}' is already registered");
        }
        /// <summary>
        /// Parses the template string and replaces placeholders with provided arguments.
        /// </summary>
        public string Parse(string _template, params string[] _args)
        {
            if (string.IsNullOrEmpty(_template))
                return _template;
            
            _template = _template.Replace("{{", LeftBracesPlaceholder.ToString());
            _template = _template.Replace("}}", RightBracesPlaceholder.ToString());

            return ParseInternal(_template, _args ?? Array.Empty<string>());
        }
        
        
        private string ParseInternal([NotNull] string _template, [NotNull] string[] _args)
        {
            StringBuilder result = new StringBuilder();
            int i = 0;

            while (i < _template.Length)
            {
                if (_template[i] == '{')
                {
                    int end = FindClosingBrace(_template, i);
                    if (end == -1)
                    {
                        result.Append(_template[i]);
                        i++;
                        continue;
                    }

                    string content = _template.Substring(i + 1, end - i - 1);
                    string resolved = ResolveContent(content, _args);
                    result.Append(resolved);
                    i = end + 1;
                }
                else
                {
                    char c = _template[i];
                    if (c == LeftBracesPlaceholder)
                        c = '{';
                    else if (c == RightBracesPlaceholder)
                        c = '}';
                    
                    result.Append(c);
                    i++;
                }
            }

            return result.ToString();
        }
        private int FindClosingBrace([NotNull] string _template, int _start)
        {
            int depth = 0;
            for (int i = _start; i < _template.Length; i++)
            {
                if (_template[i] == '{') depth++;
                else if (_template[i] == '}') depth--;

                if (depth == 0) return i;
            }

            return -1;
        }
        private string ResolveContent([NotNull] string _content, [NotNull] string[] _args)
        {
            _content = _content.Trim();
            
            StringBuilder result = new StringBuilder();
            int i = 0;
            while (i < _content.Length)
            {
                if (_content[i] == '{')
                {
                    int end = FindClosingBrace(_content, i);
                    if (end == -1)
                    {
                        result.Append(_content[i]);
                        i++;
                        continue;
                    }

                    string content = _content.Substring(i + 1, end - i - 1);
                    string resolved = ResolveContent(content, _args);
                    result.Append(resolved);
                    i = end + 1;
                }
                else
                {
                    char c = _content[i];
                    if (c == LeftBracesPlaceholder)
                        c = '{';
                    else if (c == RightBracesPlaceholder)
                        c = '}';
                    
                    result.Append(c);
                    i++;
                }
            }
            _content = result.ToString();

            if (int.TryParse(_content, out int index))
            {
                return index >= 0 && index < _args.Length ? _args[index] : $"{{{_content}}}";
            }

            int parenIndex = _content.IndexOf('(');
            if (parenIndex > 0 && _content.EndsWith(")"))
            {
                string funcName = _content.Substring(0, parenIndex).Trim();
                string argsContent = _content.Substring(parenIndex + 1, _content.Length - parenIndex - 2);

                string[] funcArgs = SplitArgs(argsContent);

                if (_m_functions.TryGetValue(funcName, out var func))
                {
                    try
                    {
                        return func(funcArgs);
                    }
                    catch
                    {
                        Console.LogWarning(SystemNames.TemplateParser, $"Function '{funcName}' execution failed");
                        return $"{{{_content}}}";
                    }
                }

                Console.LogWarning(SystemNames.TemplateParser, $"Function '{funcName}' not found");
            }

            return $"{{{_content}}}";
        }
        [NotNull]
        private string[] SplitArgs(string _argsContent)
        {
            if (string.IsNullOrEmpty(_argsContent))
                return Array.Empty<string>();
            
            string[] result = _argsContent.Split(',');
            for (int i = 0; i < result.Length; i++)
            {
                string arg = result[i];
                result[i] = arg.Trim();
            }

            return result;
        }
    }
}