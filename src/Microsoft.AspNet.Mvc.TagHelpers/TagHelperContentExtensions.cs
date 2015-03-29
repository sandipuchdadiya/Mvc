// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using System.Text;
using Microsoft.AspNet.Mvc.Razor;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;
using Microsoft.Framework.WebEncoders;

namespace Microsoft.AspNet.Mvc.TagHelpers
{
    /// <summary>
    /// Extension methods for <see cref="TagHelperContent"/>.
    /// </summary>
    public static class TagHelperContentExtensions
    {
        /// <summary>
        /// Writes the specified <paramref name="value"/> with HTML encoding to given <paramref name="content"/>.
        /// </summary>
        /// <param name="content">The <see cref="TagHelperContent"/> to write to.</param>
        /// <param name="encoder">The <see cref="IHtmlEncoder"/> to use when encoding <paramref name="value"/>.</param>
        /// <param name="encoding">The character encoding in which the <paramref name="value"/> is written.</param>
        /// <param name="value">The <see cref="object"/> to write.</param>
        /// <returns><paramref name="content"/> after the write operation has completed.</returns>
        /// <remarks>
        /// <paramref name="value"/>s of type <see cref="Rendering.HtmlString"/> are written without encoding and the
        /// <see cref="HelperResult.WriteTo(TextWriter)"/> is invoked for <see cref="HelperResult"/> types.
        /// For all other types, the encoded result of <see cref="object.ToString"/> is written to the
        /// <paramref name="writer"/>.
        /// </remarks>
        public static TagHelperContent Append(
            this TagHelperContent content,
            IHtmlEncoder encoder,
            Encoding encoding,
            object value)
        {
            using (var writer = new ContentWrapperTextWriter(content, encoding))
            {
                RazorPage.WriteTo(writer, encoder, value, escapeQuotes: true);
            }

            return content;
        }

        // Must be kept consistent with RazorPage.TagHelperContentWrapperTextWriter.
        private class ContentWrapperTextWriter : TextWriter
        {
            public ContentWrapperTextWriter(TagHelperContent content, Encoding encoding)
            {
                Content = content;
                Encoding = encoding;
            }

            public TagHelperContent Content { get; }

            public override Encoding Encoding { get; }

            public override void Write(string value)
            {
                Content.Append(value);
            }

            public override void Write(char value)
            {
                Content.Append(value.ToString());
            }

            public override string ToString()
            {
                return Content.ToString();
            }
        }
    }
}