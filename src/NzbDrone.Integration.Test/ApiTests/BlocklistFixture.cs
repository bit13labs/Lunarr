using FluentAssertions;
using NUnit.Framework;
using Lunarr.Api.V3.Movies;

namespace NzbDrone.Integration.Test.ApiTests
{
    [TestFixture]
    public class BlocklistFixture : IntegrationTest
    {
        private MovieResource _movie;

        [Test]
        [Ignore("Adding to blocklist not supported")]
        public void should_be_able_to_add_to_blocklist()
        {
            _movie = EnsureMovie(11, "The Blocklist");

            Blocklist.Post(new Lunarr.Api.V3.Blocklist.BlocklistResource
            {
                MovieId = _movie.Id,
                SourceTitle = "Blocklist.S01E01.Brought.To.You.By-BoomBoxHD"
            });
        }

        [Test]
        [Ignore("Adding to blocklist not supported")]
        public void should_be_able_to_get_all_blocklisted()
        {
            var result = Blocklist.GetPaged(0, 1000, "date", "desc");

            result.Should().NotBeNull();
            result.TotalRecords.Should().Be(1);
            result.Records.Should().NotBeNullOrEmpty();
        }

        [Test]
        [Ignore("Adding to blocklist not supported")]
        public void should_be_able_to_remove_from_blocklist()
        {
            Blocklist.Delete(1);

            var result = Blocklist.GetPaged(0, 1000, "date", "desc");

            result.Should().NotBeNull();
            result.TotalRecords.Should().Be(0);
        }
    }
}
