// Global using directives

global using System.Collections.Immutable;
global using System.Diagnostics.CodeAnalysis;
global using System.Globalization;
global using System.Net.Http.Json;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using Microsoft.Extensions.Options;
global using MongoDB.Driver;
global using Polly;
global using Polly.Retry;
global using Polly.Wrap;
global using TweetEnricher.Core.Contracts;
global using TweetEnricher.Core.Models;
global using TweetEnricher.Core.Options;
global using TweetEnricherSerializerContext = TweetEnricher.Core.Models.TweetEnricherSerializerContext;