﻿/*
 * Copyright (c) 2011-2013, Longxiang He <helongxiang@smeshlink.com>,
 * SmeshLink Technology Co.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY.
 * 
 * This file is part of the CoAP.NET, a CoAP framework in C#.
 * Please see README for more information.
 */

using System;
using System.Collections.Generic;
using System.Text;
using CoAP.Util;

namespace CoAP
{
    /// <summary>
    /// This class describes the options of the CoAP messages.
    /// </summary>
    public class Option
    {
        private static readonly IConvertor int32Convertor = new Int32Convertor();
        private static readonly IConvertor stringConvertor = new StringConvertor();

        private OptionType _type;
        /// <summary>
        /// NOTE: value bytes in network byte order (big-endian)
        /// </summary>
        private Byte[] _valueBytes;

        /// <summary>
        /// Initializes an option.
        /// </summary>
        /// <param name="type">The type of the option</param>
        protected Option(OptionType type)
        {
            this._type = type;
        }

        /// <summary>
        /// Gets the type of the option.
        /// </summary>
        public OptionType Type
        {
            get { return _type; }
        }

        /// <summary>
        /// Gets the name of the option that corresponds to its type.
        /// </summary>
        public String Name
        {
            get { return Option.ToString(_type); }
        }

        /// <summary>
        /// Gets the value's length in bytes of the option.
        /// </summary>
        public Int32 Length
        {
            get { return null == this._valueBytes ? 0 : this._valueBytes.Length; }
        }

        /// <summary>
        /// Gets or sets raw bytes value of the option in network byte order (big-endian).
        /// </summary>
        public Byte[] RawValue
        {
            get { return this._valueBytes; }
            set { this._valueBytes = value; }
        }

        /// <summary>
        /// Gets or sets string value of the option.
        /// </summary>
        public String StringValue
        {
            get
            {
                return stringConvertor.Decode(this._valueBytes) as String;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    this._valueBytes = stringConvertor.Encode(value);
            }
        }

        /// <summary>
        /// Gets or sets int value of the option.
        /// </summary>
        public Int32 IntValue
        {
            get
            {
                return (Int32)int32Convertor.Decode(this._valueBytes);
            }
            set
            {
                this._valueBytes = int32Convertor.Encode(value);
            }
        }

        /// <summary>
        /// Gets the value of the option according to its type.
        /// </summary>
        public Object Value
        {
            get
            {
                IConvertor convertor = GetConvertor(this._type);
                return null == convertor ? null : convertor.Decode(this._valueBytes);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the option has a default value according to the draft.
        /// </summary>
        public Boolean IsDefault
        {
            get
            {
                // TODO refactor
                switch (this._type)
                {
                    case OptionType.MaxAge:
                        return IntValue == CoapConstants.DefaultMaxAge;
                    case OptionType.Token:
                        return Length == 0;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Returns a human-readable string representation of the option's value.
        /// </summary>
        public override String ToString()
        {
            switch (this._type)
            {
                case OptionType.ContentType:
                    return MediaType.ToString(IntValue);
                case OptionType.MaxAge:
                    return String.Format("{0} s", IntValue);
                case OptionType.UriPort:
                case OptionType.Observe:
                case OptionType.Block2:
                case OptionType.Block1:
                case OptionType.Size:
                    return IntValue.ToString();
                case OptionType.ProxyUri:
                case OptionType.UriHost:
                case OptionType.LocationPath:
                case OptionType.LocationQuery:
                case OptionType.UriPath:
                case OptionType.UriQuery:
                    return StringValue;
                case OptionType.IfNoneMatch:
                    return "set";
                case OptionType.Accept:
                    return MediaType.ToString(IntValue);
                case OptionType.ETag:
                case OptionType.Token:
                case OptionType.IfMatch:
                default:
                    return ByteArrayUtils.ToHexString(RawValue);
            }
        }

        /// <summary>
        /// Gets the hash code of this object
        /// </summary>
        /// <returns>The hash code</returns>
        public override Int32 GetHashCode()
        {
            const Int32 prime = 31;
            Int32 result = 1;
            result = prime * result + (Int32)this._type;
            result = prime * result + ComputeHash(this.RawValue);
            return result;
        }

        public override Boolean Equals(Object obj)
        {
            if (null == obj)
                return false;
            if (Object.ReferenceEquals(this, obj))
                return true;
            if (this.GetType() != obj.GetType())
                return false;
            Option other = (Option)obj;
            if (this._type != other._type)
                return false;
            if (null == this.RawValue && null != other.RawValue)
                return false;
            else if (null != this.RawValue && null == other.RawValue)
                return false;
            else
                return Sort.IsSequenceEqualTo(this.RawValue, other.RawValue);
        }

        /// <summary>
        /// Creates an option.
        /// </summary>
        /// <param name="type">The type of the option</param>
        /// <returns>The new option</returns>
        public static Option Create(OptionType type)
        {
            switch (type)
            {
                case OptionType.Block1:
                case OptionType.Block2:
                    return new BlockOption(type);
                default:
                    return new Option(type);
            }
        }

        /// <summary>
        /// Creates an option.
        /// </summary>
        /// <param name="type">The type of the option</param>
        /// <param name="raw">The raw bytes value of the option</param>
        /// <returns>The new option</returns>
        public static Option Create(OptionType type, Byte[] raw)
        {
            Option opt = Create(type);
            opt.RawValue = raw;
            return opt;
        }

        /// <summary>
        /// Creates an option.
        /// </summary>
        /// <param name="type">The type of the option</param>
        /// <param name="str">The string value of the option</param>
        /// <returns>The new option</returns>
        public static Option Create(OptionType type, String str)
        {
            Option opt = Create(type);
            opt.StringValue = str;
            return opt;
        }

        /// <summary>
        /// Creates an option.
        /// </summary>
        /// <param name="type">The type of the option</param>
        /// <param name="val">The int value of the option</param>
        /// <returns>The new option</returns>
        public static Option Create(OptionType type, Int32 val)
        {
            Option opt = Create(type);
            opt.IntValue = val;
            return opt;
        }

        /// <summary>
        /// Splits a string into a set of options, e.g. a uri path.
        /// </summary>
        /// <param name="type">The type of options</param>
        /// <param name="s">The string to be splited</param>
        /// <param name="delimiter">The seperator string</param>
        /// <returns><see cref="System.Collections.Generic.IEnumerable&lt;T&gt;"/> of options</returns>
        public static IEnumerable<Option> Split(OptionType type, String s, String delimiter)
        {
            List<Option> opts = new List<Option>();
            if (!String.IsNullOrEmpty(s))
            {
                while (s.StartsWith(delimiter))
                {
                    s = s.Substring(delimiter.Length);
                }

                foreach (String segment in s.Split(new String[] { delimiter }, StringSplitOptions.None))
                {
                    // empty path segments are allowed (e.g., /test vs /test/)
                    if ("/".Equals(delimiter) || !String.IsNullOrEmpty(segment))
                    {
                        opts.Add(Create(type, segment));
                    }
                }
            }
            return opts;
        }

        /// <summary>
        /// Joins the string values of a set of options.
        /// </summary>
        /// <param name="options">The list of options to be joined</param>
        /// <param name="delimiter">The seperator string</param>
        /// <returns>The joined string</returns>
        public static String Join(IEnumerable<Option> options, String delimiter)
        {
            if (null == options)
            {
                return String.Empty;
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                Boolean append = false;
                foreach (Option opt in options)
                {
                    if (append)
                        sb.Append(delimiter);
                    else
                        append = true;
                    sb.Append(opt.StringValue);
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Returns a string representation of the option type.
        /// </summary>
        /// <param name="type">The option type to describe</param>
        /// <returns>A string describing the option type</returns>
        public static String ToString(OptionType type)
        {
            switch (type)
            {
                case OptionType.Reserved:
                    return "Reserved (0)";
                case OptionType.ContentType:
                    return "Content-Type";
                case OptionType.MaxAge:
                    return "Max-Age";
                case OptionType.ProxyUri:
                    return "Proxy-Uri";
                case OptionType.ETag:
                    return "ETag";
                case OptionType.UriHost:
                    return "Uri-Host";
                case OptionType.LocationPath:
                    return "Location-Path";
                case OptionType.UriPort:
                    return "Uri-Port";
                case OptionType.LocationQuery:
                    return "Location-Query";
                case OptionType.UriPath:
                    return "Uri-Path";
                case OptionType.Token:
                    return "Token";
                case OptionType.UriQuery:
                    return "Uri-Query";
                case OptionType.Observe:
                    return "Observe";
                case OptionType.Accept:
                    return "Accept";
                case OptionType.IfMatch:
                    return "If-Match";
                case OptionType.FencepostDivisor:
                    return "Fencepost-Divisor";
                case OptionType.Block2:
                    return "Block2";
                case OptionType.Block1:
                    return "Block1";
                case OptionType.Size:
                    return "Size";
                case OptionType.IfNoneMatch:
                    return "If-None-Match";
                case OptionType.ProxyScheme:
                    return "Proxy-Scheme";
                default:
                    return String.Format("Unknown option [{0}]", type);
            }
        }

        public static OptionFormat GetFormatByType(OptionType type)
        {
            switch (type)
            {
                case OptionType.ContentType:
                case OptionType.MaxAge:
                case OptionType.UriPort:
                case OptionType.Observe:
                case OptionType.Block2:
                case OptionType.Block1:
                case OptionType.Accept:
                case OptionType.FencepostDivisor:
                case OptionType.IfNoneMatch:
                    return OptionFormat.Integer;
                case OptionType.ProxyUri:
                case OptionType.UriHost:
                case OptionType.LocationPath:
                case OptionType.LocationQuery:
                case OptionType.UriPath:
                case OptionType.Token:
                case OptionType.UriQuery:
                    return OptionFormat.String;
                case OptionType.ETag:
                case OptionType.IfMatch:
                    return OptionFormat.Opaque;
                default:
                    return OptionFormat.Unknown;
            }
        }

        private static Int32 ComputeHash(params Byte[] data)
        {
            unchecked
            {
                const Int32 p = 16777619;
                Int32 hash = (Int32)2166136261;

                for (Int32 i = 0; i < data.Length; i++)
                    hash = (hash ^ data[i]) * p;

                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;
                return hash;
            }
        }

        private static IConvertor GetConvertor(OptionType type)
        {
            switch (type)
            {
                case OptionType.Reserved:
                    return null;
                case OptionType.ContentType:
                case OptionType.MaxAge:
                case OptionType.UriPort:
                case OptionType.Observe:
                case OptionType.Block2:
                case OptionType.Block1:
                case OptionType.Accept:
                case OptionType.FencepostDivisor:
                    return int32Convertor;
                case OptionType.ProxyUri:
                case OptionType.ETag:
                case OptionType.UriHost:
                case OptionType.LocationPath:
                case OptionType.LocationQuery:
                case OptionType.UriPath:
                case OptionType.Token:
                case OptionType.UriQuery:
                case OptionType.IfMatch:
                case OptionType.IfNoneMatch:
                    return stringConvertor;
                default:
                    return null;
            }
        }

        interface IConvertor
        {
            Object Decode(Byte[] bytes);
            Byte[] Encode(Int32 value);
            Byte[] Encode(String value);
        }

        class Int32Convertor : IConvertor
        {
            public Object Decode(Byte[] bytes)
            {
                if (null == bytes)
                    return 0;

                Int32 iOutcome = 0;
                Byte bLoop;
                for (Int32 i = 0; i < bytes.Length; i++)
                {
                    bLoop = bytes[i];
                    //iOutcome |= (bLoop & 0xFF) << (8 * i);
                    iOutcome <<= 8;
                    iOutcome |= (bLoop & 0xFF);
                }
                return iOutcome;
            }

            public Byte[] Encode(Int32 value)
            {
                Byte[] ret;

                if (value == 0)
                {
                    ret = new Byte[1];
                    ret[0] = 0;
                    return ret;
                }

                Int32 val = System.Net.IPAddress.HostToNetworkOrder(value);
                Byte[] allBytes = BitConverter.GetBytes(val);
                Int32 neededBytes = allBytes.Length;
                //for (Int32 i = allBytes.Length - 1; i >= 0; i--)
                for (Int32 i = 0; i < allBytes.Length; i++)
                {
                    if (allBytes[i] == 0x00)
                        neededBytes--;
                    else
                        break;
                }
                if (neededBytes == allBytes.Length)
                    ret = allBytes;
                else
                {
                    ret = new Byte[neededBytes];
                    Array.Copy(allBytes, allBytes.Length - neededBytes, ret, 0, neededBytes);
                }

                return ret;
            }

            public Byte[] Encode(String value)
            {
                throw new NotSupportedException();
            }
        }

        class StringConvertor : IConvertor
        {
            public Object Decode(Byte[] bytes)
            {
                return null == bytes ? null : System.Text.Encoding.UTF8.GetString(bytes);
            }

            public Byte[] Encode(String value)
            {
                return System.Text.Encoding.UTF8.GetBytes(value);
            }

            public Byte[] Encode(Int32 value)
            {
                throw new NotSupportedException();
            }
        }
    }
}
