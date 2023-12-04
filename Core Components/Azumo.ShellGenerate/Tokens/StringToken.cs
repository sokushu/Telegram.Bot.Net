﻿namespace Azumo.ShellGenerate.Tokens
{
    public class StringToken : TokenBase
    {
        private readonly string _value;
        public StringToken(string text)
        {
            _value = text;
        }

        public override string Generate()
        {
            return _value;
        }

        public override TokenBase Param(TokenBase token)
        {
            return this;
        }
    }
}
