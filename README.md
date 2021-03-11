# Ipc
Testing different IPC's in C# in different versions with different test sizes.

Keep in mind that this wasn't really a realistic test. All tests were done by with the goal in mind of achieving maximum calls per second. I just had a hot loop on both the server and client running as fast as possible.
Also keep in mind I may have totally messed up my performance testing and it is all lies.

Testing on your machine will doubtless result in different numbers, but I would expect the relative numbers to be the same. At least, on Windows 10.

The testing scenario was that the server would start, the client would start and connect, then both would try to get as much through as possible in 60 seconds. Every second the client would output how many calls to read it had made. The first result and the last result were thrown away due to "warm-up" time and sometimes errornous results at the end.

Raw results are stored in <a href="https://github.com/asajs/Ipc/tree/master/Results">/results</a> in both .txt and .csv. Aggregated average values are stored in <a href="https://github.com/asajs/Ipc/tree/master/Results/Parsed">/results/parsed</a> as .csv files

The last four rows of the raw data .csv files are average, standard devation, minimum, and maximum, in order.

Tested across .NET Framework 4.7.2, .NET 5.0, .NET Core 3.1

IPC's tested: UDP, Async UDP, TCP, Async TCP, Pipes, Async Pipes, Memory Sharing, Memory Sharing with Mutexes

Byte sizes tested: 19, 108, 59892<sup>1</sup>

Key takeaways
 1. .NET 5.0 is <i>almost</i> always better.
 2. Syncronyous pipes don't scale well with payload size.
 3. Memory sharing without mutexes is blazing fast.
 4. Memory sharing with mutexes is shockingly slow in comparision.
 5. TCP is often better than pipes in .NET 5.0 and .NET Core 3.1.
 6. If you have a small payload TCP performs better than UDP. If you have a large payload UDP performs better than TCP.
 7. Async consumes far more CPU and offers worse performance than the syncronous version for all circumstances except asyncronous pipes with a large payload.
    The caveat with this is that if you have an infrequent<sup>2</sup> message it may be more effective to have an async methodolgy as a thread is not being blocked waiting for a payload.

<sup>1</sup> How come I didn't use byte sizes that are a power of 2? Because that wasn't my real use case scenario.    
<sup>2</sup> What is considered infrequent? This depends on a lot of things, including how much work you are doing per payload. But as a rough guideline, it is hard to imagine that there is much benefit to be gained by asyncronous methodology if messages are coming faster than 10-15 ms. Or, in other words, messages that come every 10-15 ms can be considered "frequent"
