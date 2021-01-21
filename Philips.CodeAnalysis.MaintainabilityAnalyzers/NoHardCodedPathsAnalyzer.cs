﻿using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Philips.CodeAnalysis.Common;

namespace Philips.CodeAnalysis.MaintainabilityAnalyzers
{
	/*
	 * Analyzer for hardcoded absolute path. 
	 * Reports diagnostics if an absolute path is used, for example: c:\users\Bin\example.xml.
	 */
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class NoHardCodedPathsAnalyzer : DiagnosticAnalyzer
	{

		#region Non-Public Data Members
		private const string Title = @"Paths must not be hardcoded";
		private const string MessageFormat = @"Paths must not be hardcoded";
		private const string Description = @"Paths must not be hardcoded";
		private const string Category = Categories.Maintainability;
		private Regex Pattern = new Regex(@"^[a-zA-Z]:\\(((?![<>:/\\|?*]).)+((?<![ .])\\)?)*$");
		#endregion

		#region Non-Public Data Members
		private void Analyze(SyntaxNodeAnalysisContext context)
		{
			LiteralExpressionSyntax stringLiteralExpressionNode = (LiteralExpressionSyntax)context.Node;
			// Get the text value of the literal expression.
			string pathValue = stringLiteralExpressionNode.Token.ValueText;
			// If the pattern matches the text value, report the diagnostic.
			if (Pattern.IsMatch(pathValue))
			{
				Diagnostic diagnostic = Diagnostic.Create(Rule, stringLiteralExpressionNode.GetLocation());
				context.ReportDiagnostic(diagnostic);
			}
		}
		#endregion

		#region Public Interface
		public static DiagnosticDescriptor Rule = new DiagnosticDescriptor(Helper.ToDiagnosticId(DiagnosticIds.NoHardcodedPaths), Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
		{
			get { return ImmutableArray.Create(Rule); }
		}
		public override void Initialize(AnalysisContext context)
		{
			context.EnableConcurrentExecution();
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.StringLiteralExpression);
		}
		#endregion

	}
}
