// Global using directives

global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;

global using Polly;
global using Polly.Retry;
global using Polly.Wrap;

global using System.Collections.Immutable;
global using System.Net.Http.Json;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using MongoDB.Driver;
global using TweetEnricher.Clients;
global using TweetEnricher.Contracts;
global using TweetEnricher.Models;
global using TweetEnricher.Options;
global using TweetEnricher.Services;

global using static System.Environment;