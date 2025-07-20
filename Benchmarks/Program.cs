// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using Benchmarks;

BenchmarkRunner.Run<MikrotikApiBenchmark>();

// var x = new MikrotikApiBenchmark();

// var x1 = await x.GetInterfacesOriginal();
// var x2 = await x.GetInterfacesOptimized();
// var x3 = await x.GetInterfacesTurboOptimized();
//
// int xd = 2;