﻿/*
 * TrustAgent.Hex.cs 
 * Developer: Andrew Savinykh
 * Source: https://codereview.stackexchange.com/q/145506
 * Developement stage: Completed
 * Tested on: macOS Mojave (10.14.1) -> PASSED
 * 
 * Generates a string containing an hex dump of byte array
 * 
 * Requires initialization: YES
 * Contains:
 *     Class Level Variables: 6 Private (Read Only), 1 Private
 *     Inner Classes: 1 Public
 *     Enums: 6 Private
 *     Methods:
 *         Static: 1 Public
 *         Non Static: 7 Private
 * 
 */

using System;
using System.Text;

namespace TrustAgent
{
    public class Hex
    {
        readonly byte[] _bytes;
        readonly int _bytesPerLine;
        readonly bool _showHeader;
        readonly bool _showOffset;
        readonly bool _showAscii;

        readonly int _length;

        int _index;
        readonly StringBuilder _sb = new StringBuilder();

        public Hex(byte[] bytes, int bytesPerLine, bool showHeader, bool showOffset, bool showAscii)
        {
            _bytes = bytes;
            _bytesPerLine = bytesPerLine;
            _showHeader = showHeader;
            _showOffset = showOffset;
            _showAscii = showAscii;
            _length = bytes.Length;
        }

        public static string Dump(byte[] bytes, int bytesPerLine = 16, bool showHeader = true, bool showOffset = true, bool showAscii = true)
        {
            return bytes == null ? "<null>" : (new Hex(bytes, bytesPerLine, showHeader, showOffset, showAscii)).Dump();
        }

        string Dump()
        {
            if (_showHeader)
            {
                WriteHeader();
            }
            WriteBody();
            return _sb.ToString();
        }

        void WriteHeader()
        {
            if (_showOffset)
            {
                _sb.Append("Offset(h)  ");
            }
            for (int i = 0; i < _bytesPerLine; i++)
            {
                _sb.Append($"{i & 0xFF:X2}");
                if (i + 1 < _bytesPerLine)
                {
                    _sb.Append(" ");
                }
            }
            _sb.AppendLine();
        }

        void WriteBody()
        {
            while (_index < _length)
            {
                if (_index % _bytesPerLine == 0)
                {
                    if (_index > 0)
                    {
                        if (_showAscii)
                        {
                            WriteAscii();
                        }
                        _sb.AppendLine();
                    }

                    if (_showOffset)
                    {
                        WriteOffset();
                    }
                }

                WriteByte();
                if (_index % _bytesPerLine != 0 && _index < _length)
                {
                    _sb.Append(" ");
                }
            }

            if (_showAscii)
            {
                WriteAscii();
            }
        }

        void WriteOffset()
        {
            _sb.Append($"{_index:X8}   ");
        }

        void WriteByte()
        {
            _sb.Append($"{_bytes[_index]:X2}");
            _index++;
        }

        void WriteAscii()
        {
            int backtrack = ((_index - 1) / _bytesPerLine) * _bytesPerLine;
            int length = _index - backtrack;

            // This is to fill up last string of the dump if it's shorter than _bytesPerLine
            _sb.Append(new string(' ', (_bytesPerLine - length) * 3));

            _sb.Append("   ");
            for (int i = 0; i < length; i++)
            {
                _sb.Append(Translate(_bytes[backtrack + i]));
            }
        }

        string Translate(byte b)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            return b < 32 ? "." : Encoding.GetEncoding(1252).GetString(new[] { b });
        }
    }
}
