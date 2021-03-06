/*
 * Copyright 2007-2011 JetBrains s.r.o.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using JetBrains.TeamCity.ServiceMessages.Write.Special;
using JetBrains.TeamCity.ServiceMessages.Write.Special.Impl.Writer;
using NUnit.Framework;

namespace JetBrains.TeamCity.ServiceMessages.Tests.Write
{
  [TestFixture]
  public class TeamCityTestsWriterTest : TeamCityWriterBaseTest<ITeamCityTestsWriter>
  {
    protected override ITeamCityTestsWriter Create(IServiceMessageProcessor proc)
    {
      return new TeamCityTestsWriter(proc);
    }

    [Test]
    public void TestSuiteOpenClose()
    {
      DoTest(x => x.OpenTestSuite("suite3").Dispose(), "##teamcity[testSuiteStarted name='suite3']", "##teamcity[testSuiteFinished name='suite3']");
    }


    [Test, ExpectedException]
    public void TestDoNotLetOpenTwoTests()
    {
      DoTest(x =>
               {
                 x.OpenTest("test1");
                 x.OpenTest("test23");
               });
    }

    [Test, ExpectedException]
    public void TestDoNotLetOpenTwoSuites()
    {
      DoTest(x =>
               {
                 x.OpenTestSuite("test1");
                 x.OpenTestSuite("test23");
               });
    }

    [Test, ExpectedException]
    public void TestDoNotLetOpenTwoTestNSuites()
    {
      DoTest(x =>
               {
                 x.OpenTest("test1");
                 x.OpenTestSuite("test1").OpenTest("z3");                 
               });
    }

    [Test]
    public void TestSuiteInSuite()
    {
      DoTest(x =>
               {
                 using (var suite = x.OpenTestSuite("suite3"))
                 {
                   using (var suite2 = suite.OpenTestSuite("suite3.444")) ;
                 }
               },
             "##teamcity[testSuiteStarted name='suite3']",
             "##teamcity[testSuiteStarted name='suite3.444']",
             "##teamcity[testSuiteFinished name='suite3.444']",
             "##teamcity[testSuiteFinished name='suite3']"
        );
    }


    [Test]
    public void TestSuiteInSuiteTest()
    {
      DoTest(x =>
               {
                 using (var suite = x.OpenTestSuite("suite3"))
                 {
                   using (var suite2 = suite.OpenTestSuite("suite3.333"))
                   {
                     using (var test = suite2.OpenTest("test")) ;
                   }
                 }
               },
             "##teamcity[testSuiteStarted name='suite3']",
             "##teamcity[testSuiteStarted name='suite3.333']",
             "##teamcity[testStarted name='test' captureStandardOutput='false']",
             "##teamcity[testFinished name='test']",
             "##teamcity[testSuiteFinished name='suite3.333']",
             "##teamcity[testSuiteFinished name='suite3']"
        );
    }

    [Test]
    public void TestSuiteInSuiteTest2()
    {
      DoTest(x =>
               {
                 using (var suite = x.OpenTestSuite("suite3"))
                 {
                   using (var suite2 = suite.OpenTestSuite("suite3.333"))
                   {
                     using (var test = suite2.OpenTest("test")) ;
                     using (var test = suite2.OpenTest("test2")) ;
                     using (var test = suite2.OpenTest("test3")) ;
                   }
                 }
               },
             "##teamcity[testSuiteStarted name='suite3']",
             "##teamcity[testSuiteStarted name='suite3.333']",
             "##teamcity[testStarted name='test' captureStandardOutput='false']",
             "##teamcity[testFinished name='test']",
             "##teamcity[testStarted name='test2' captureStandardOutput='false']",
             "##teamcity[testFinished name='test2']",
             "##teamcity[testStarted name='test3' captureStandardOutput='false']",
             "##teamcity[testFinished name='test3']",
             "##teamcity[testSuiteFinished name='suite3.333']",
             "##teamcity[testSuiteFinished name='suite3']"
        );
    }

    [Test]
    public void TestSuiteInSuite2Test2()
    {
      DoTest(x =>
               {
                 using (var suite = x.OpenTestSuite("suite3"))
                 {
                   using (var suite2 = suite.OpenTestSuite("suite3.333"))
                   {
                     using (var test = suite2.OpenTest("test")) ;
                     using (var test = suite2.OpenTest("test2")) ;
                     using (var test = suite2.OpenTest("test3")) ;
                   }
                   using (var suite2 = suite.OpenTestSuite("suite2.444"))
                   {
                     using (var test = suite2.OpenTest("testZ")) ;
                   }
                 }
               },
             "##teamcity[testSuiteStarted name='suite3']",
             "##teamcity[testSuiteStarted name='suite3.333']",
             "##teamcity[testStarted name='test' captureStandardOutput='false']",
             "##teamcity[testFinished name='test']",
             "##teamcity[testStarted name='test2' captureStandardOutput='false']",
             "##teamcity[testFinished name='test2']",
             "##teamcity[testStarted name='test3' captureStandardOutput='false']",
             "##teamcity[testFinished name='test3']",
             "##teamcity[testSuiteFinished name='suite3.333']",
             "##teamcity[testSuiteStarted name='suite2.444']",
             "##teamcity[testStarted name='testZ' captureStandardOutput='false']",
             "##teamcity[testFinished name='testZ']",
             "##teamcity[testSuiteFinished name='suite2.444']",
             "##teamcity[testSuiteFinished name='suite3']"
        );
    }

    [Test]
    public void TestTestOpenClose()
    {
      DoTest(x => x.OpenTest("suite3").Dispose(), "##teamcity[testStarted name='suite3' captureStandardOutput='false']", "##teamcity[testFinished name='suite3']");
    }

    [Test]
    public void TestTestOutput()
    {
      DoTest(x =>
               {
                 using (var test = x.OpenTest("test"))
                 {
                   test.WriteStdOutput("outp4ut");
                 }
               },
             "##teamcity[testStarted name='test' captureStandardOutput='false']",
             "##teamcity[testStdOut name='test' out='outp4ut']",
             "##teamcity[testFinished name='test']"
        );
    }

    [Test]
    public void TestTestError()
    {
      DoTest(x =>
               {
                 using (var test = x.OpenTest("test"))
                 {
                   test.WriteErrOutput("error3");
                 }
               },
             "##teamcity[testStarted name='test' captureStandardOutput='false']", 
             "##teamcity[testStdErr name='test' out='error3']",
             "##teamcity[testFinished name='test']");
    }

    [Test]
    public void TestTestIgnored()
    {
      DoTest(x =>
               {
                 using (var test = x.OpenTest("test"))
                 {
                   test.WriteIgnored();
                 }
               },
             "##teamcity[testStarted name='test' captureStandardOutput='false']",
             "##teamcity[testIgnored name='test']",
             "##teamcity[testFinished name='test']");
    }

    [Test]
    public void TestTestIgnoredWithMessage()
    {
      DoTest(x =>
               {
                 using (var test = x.OpenTest("test"))
                 {
                   test.WriteIgnored("some reason");
                 }
               },
             "##teamcity[testStarted name='test' captureStandardOutput='false']",
             "##teamcity[testIgnored name='test' message='some reason']",
             "##teamcity[testFinished name='test']");
    }

    [Test]
    public void TestTestFailed()
    {
      DoTest(x =>
               {
                 using (var test = x.OpenTest("test5"))
                 {
                   test.WriteFailed("some reason", "details");
                 }
               },
             "##teamcity[testStarted name='test5' captureStandardOutput='false']", 
             "##teamcity[testFailed name='test5' message='some reason' details='details']",
             "##teamcity[testFinished name='test5']");
    }

    [Test]
    public void TestTestDuration()
    {
      DoTest(x =>
               {
                 using (var test = x.OpenTest("test"))
                 {
                   test.WriteDuration(TimeSpan.FromMilliseconds(1000));
                 }
               },
             "##teamcity[testStarted name='test' captureStandardOutput='false']",
             "##teamcity[testFinished name='test' duration='1000']");

    }

  }
}