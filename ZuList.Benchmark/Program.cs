// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using System.Diagnostics;
using ZuList;
using ZuList.Benchmark;

var switcher = new BenchmarkSwitcher(new[] { typeof(Benchmark) });
switcher.Run(new string[] { "Release", "--filter","*" });