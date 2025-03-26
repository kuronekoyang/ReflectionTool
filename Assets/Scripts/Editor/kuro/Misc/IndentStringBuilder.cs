using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace kuro.Core
{
    public class IndentStringBuilder
    {
        private readonly StringBuilder _stringBuilder;
        private int _indent = 0;
        private int _disableIndent = 0;
        private int _lineCount = 0;

        public int Length => _stringBuilder?.Length ?? 0;
        public int LineCount => _lineCount;
        public StringBuilder InternalStringBuilder => _stringBuilder;

        public IndentStringBuilder()
        {
            _stringBuilder = new();
        }

        public IndentStringBuilder(int capacity)
        {
            _stringBuilder = new(capacity);
        }

        public IndentStringBuilder(IndentStringBuilder baseSb)
        {
            _stringBuilder = new();
            this._indent = baseSb._indent;
            this._disableIndent = baseSb._disableIndent;
        }

        public string IndentString => new(' ', _indent);
        public void IncreaseIndent(int value) => _indent += value;

        public readonly struct DisableIndentScopeImpl : IDisposable
        {
            private readonly IndentStringBuilder _stringBuilder;

            public DisableIndentScopeImpl(IndentStringBuilder sb)
            {
                this._stringBuilder = sb;
                ++sb._disableIndent;
            }

            public void Dispose()
            {
                --_stringBuilder._disableIndent;
            }
        }

        public DisableIndentScopeImpl DisableIndent() => new DisableIndentScopeImpl(this);

        public readonly struct IndentScopeImpl : IDisposable
        {
            private readonly IndentStringBuilder _stringBuilder;

            public IndentScopeImpl(IndentStringBuilder sb)
            {
                this._stringBuilder = sb;
                ++sb._indent;
            }

            public void Dispose()
            {
                --_stringBuilder._indent;
            }
        }

        public IndentScopeImpl Indent() => new IndentScopeImpl(this);

        public void Clear()
        {
            this._stringBuilder.Clear();
            this._indent = 0;
            this._disableIndent = 0;
        }


        private void AppendIndentImpl()
        {
            if (_disableIndent > 0 || _indent <= 0)
                return;
            this._stringBuilder.Append(' ', _indent * 4);
        }

        public IndentStringBuilder AppendLine(string text = "")
        {
            ++_lineCount;
            if (!string.IsNullOrEmpty(text))
                this.AppendIndentImpl();
            this._stringBuilder.AppendLine(text);
            return this;
        }

        public IndentStringBuilder BeginLine(string text = "")
        {
            this.AppendIndentImpl();
            this._stringBuilder.Append(text);
            return this;
        }

        public IndentStringBuilder EndLine(string text = "")
        {
            ++_lineCount;
            this._stringBuilder.AppendLine(text);
            return this;
        }

        public unsafe IndentStringBuilder Append(char* value, int charCount)
        {
            this._stringBuilder.Append(value, charCount);
            return this;
        }

        public IndentStringBuilder Append(char[] value, int startIndex, int charCount)
        {
            this._stringBuilder.Append(value, startIndex, charCount);
            return this;
        }

        public IndentStringBuilder Append(string value)
        {
            this._stringBuilder.Append(value);
            return this;
        }

        public IndentStringBuilder Append(string value, int startIndex, int count)
        {
            this._stringBuilder.Append(value, startIndex, count);
            return this;
        }

        public IndentStringBuilder Append(StringBuilder value)
        {
            this._stringBuilder.Append(value);
            return this;
        }

        public IndentStringBuilder Append(StringBuilder? value, int startIndex, int count)
        {
            if (value != null)
            {
                for (int i = 0; i < count; ++i)
                    this._stringBuilder.Append(value[startIndex + i]);
            }

            return this;
        }

        public IndentStringBuilder Append(IndentStringBuilder value)
        {
            if (value?._stringBuilder != null)
                this._stringBuilder.Append(value?._stringBuilder);
            return this;
        }

        public IndentStringBuilder Append(IndentStringBuilder value, int startIndex, int count)
        {
            this.Append(value?._stringBuilder, startIndex, count);
            return this;
        }

        public IndentStringBuilder Append(bool value)
        {
            this._stringBuilder.Append(value);
            return this;
        }

        public IndentStringBuilder Append(char value)
        {
            this._stringBuilder.Append(value);
            return this;
        }

        public IndentStringBuilder Append(char value, int count)
        {
            this._stringBuilder.Append(value, count);
            return this;
        }

        public IndentStringBuilder Append(sbyte value)
        {
            this._stringBuilder.Append(value);
            return this;
        }

        public IndentStringBuilder Append(byte value)
        {
            this._stringBuilder.Append(value);
            return this;
        }

        public IndentStringBuilder Append(short value)
        {
            this._stringBuilder.Append(value);
            return this;
        }

        public IndentStringBuilder Append(int value)
        {
            this._stringBuilder.Append(value);
            return this;
        }

        public IndentStringBuilder Append(long value)
        {
            this._stringBuilder.Append(value);
            return this;
        }

        public IndentStringBuilder Append(float value)
        {
            this._stringBuilder.Append(value);
            return this;
        }

        public IndentStringBuilder Append(double value)
        {
            this._stringBuilder.Append(value);
            return this;
        }

        public IndentStringBuilder Append(Decimal value)
        {
            this._stringBuilder.Append(value);
            return this;
        }

        public IndentStringBuilder Append(ushort value)
        {
            this._stringBuilder.Append(value);
            return this;
        }

        public IndentStringBuilder Append(uint value)
        {
            this._stringBuilder.Append(value);
            return this;
        }

        public IndentStringBuilder Append(ulong value)
        {
            this._stringBuilder.Append(value);
            return this;
        }

        public IndentStringBuilder Append(object value)
        {
            this._stringBuilder.Append(value);
            return this;
        }

        public IndentStringBuilder Append(ReadOnlySpan<char> value)
        {
            this._stringBuilder.Append(value);
            return this;
        }

        private void CommentImpl(string comment)
        {
            if (string.IsNullOrEmpty(comment))
                return;

            using (StringReader sr = new StringReader(comment))
            {
                string? line;
                while ((line = sr?.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(line))
                        this.AppendLine($"/// {line}");
                }
            }
        }

        public IndentStringBuilder Comment(string comment1)
        {
            if (string.IsNullOrEmpty(comment1))
                return this;
            this.AppendLine("/// <summary>");
            this.CommentImpl(comment1);
            this.AppendLine("/// </summary>");
            return this;
        }

        public override string ToString() => this._stringBuilder.ToString() ?? "";
    }
}