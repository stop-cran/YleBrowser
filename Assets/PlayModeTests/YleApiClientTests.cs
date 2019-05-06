using Assets.Scripts;
using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;

namespace Tests
{
    public class YleApiClientTests
    {
        private YleApiClient searcher;

        [SetUp]
        public void Setup()
        {
            searcher = new YleApiClient(
                "https://external.api.yle.fi",
                "c6d65741",
                "6a8b448b276a88171bad6f22a059b6cd");
        }

        [UnityTest]
        public IEnumerator ShouldSucceed()
        {
            yield return searcher.Search("muumi", 0, 1);

            Assert.IsTrue(searcher.Success,
                "The request should succeed");
        }

        [UnityTest]
        public IEnumerator ShouldReturnNoResultOnNonsenseQuery()
        {
            yield return searcher.Search("qqqqqqq", 0, 1);

            Assert.IsTrue(searcher.Success,
                "The request should succeed");
            Assert.AreEqual(searcher.Result.Count, 0,
                "The result should not be empty.");
        }

        [UnityTest]
        public IEnumerator ShouldReturnResult()
        {
            yield return searcher.Search("muumi", 0, 1);

            Assert.AreEqual(searcher.Result.Count, 1,
                "The result should not be empty.");

            Assert.IsNotNull(searcher.Result[0],
                "The result item should not be empty");

            Assert.IsNotNull(searcher.Result[0].title,
                "The result item title should not be empty");

            Assert.Greater(searcher.Result[0].title.Length, 0,
                "The result item title should not be empty");
        }
    }
}
