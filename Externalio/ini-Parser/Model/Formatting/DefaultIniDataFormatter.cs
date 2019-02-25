using System;
using System.Collections.Generic;
using System.Text;
using Externalio.Model.Configuration;

namespace Externalio.Model.Formatting
{
	public class DefaultIniDataFormatter : IIniDataFormatter
	{
		private IniParserConfiguration _configuration;

		public virtual string IniDataToString(IniData iniData)
		{
			var sb = new StringBuilder();

			if (Configuration.AllowKeysWithoutSection)
				WriteKeyValueData(iniData.Global, sb);

			//Write sections
			foreach (var section in iniData.Sections)
				//Write current section
				WriteSection(section, sb);

			return sb.ToString();
		}

		/// <summary>
		///     Configuration used to write an ini file with the proper
		///     delimiter characters and data.
		/// </summary>
		/// <remarks>
		///     If the <see cref="IniData" /> instance was created by a parser,
		///     this instance is a copy of the <see cref="IniParserConfiguration" /> used
		///     by the parser (i.e. different objects instances)
		///     If this instance is created programatically without using a parser, this
		///     property returns an instance of <see cref=" IniParserConfiguration" />
		/// </remarks>
		public IniParserConfiguration Configuration
		{
			get => _configuration;
			set => _configuration = value.Clone();
		}

		#region Initialization

		public DefaultIniDataFormatter() : this(new IniParserConfiguration())
		{
		}

		public DefaultIniDataFormatter(IniParserConfiguration configuration)
		{
			if (configuration == null)
				throw new ArgumentNullException("configuration");
			Configuration = configuration;
		}

		#endregion

		#region Helpers

		private void WriteSection(SectionData section, StringBuilder sb)
		{
			// Write blank line before section, but not if it is the first line
			if (sb.Length > 0) sb.Append(Configuration.NewLineStr);

			// Leading comments
#pragma warning disable CS0618 // Typ oder Element ist veraltet
			WriteComments(section.LeadingComments, sb);
#pragma warning restore CS0618 // Typ oder Element ist veraltet

			//Write section name
			sb.Append(string.Format("{0}{1}{2}{3}",
				Configuration.SectionStartChar,
				section.SectionName,
				Configuration.SectionEndChar,
				Configuration.NewLineStr));

			WriteKeyValueData(section.Keys, sb);

			// Trailing comments
#pragma warning disable CS0618 // Typ oder Element ist veraltet
			WriteComments(section.TrailingComments, sb);
#pragma warning restore CS0618 // Typ oder Element ist veraltet
		}

		private void WriteKeyValueData(KeyDataCollection keyDataCollection, StringBuilder sb)
		{
			foreach (var keyData in keyDataCollection)
			{
				// Add a blank line if the key value pair has comments
				if (keyData.Comments.Count > 0) sb.Append(Configuration.NewLineStr);

				// Write key comments
				WriteComments(keyData.Comments, sb);

				//Write key and value
				sb.Append(string.Format("{0}{3}{1}{3}{2}{4}",
					keyData.KeyName,
					Configuration.KeyValueAssigmentChar,
					keyData.Value,
					Configuration.AssigmentSpacer,
					Configuration.NewLineStr));
			}
		}

		private void WriteComments(List<string> comments, StringBuilder sb)
		{
			foreach (var comment in comments)
				sb.Append(string.Format("{0}{1}{2}", Configuration.CommentString, comment, Configuration.NewLineStr));
		}

		#endregion
	}
}