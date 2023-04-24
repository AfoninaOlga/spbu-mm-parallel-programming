namespace ProducerConsumer.Tests
{
    public class ProducerConsumerTests
    {
        [Test]
        public void SmokeTest()
        {
            var _algorithm = new ProducerConsumerAlgorithm<int>(1, 1, () => 5, true);
            _algorithm.Run();
        }

        [Test]
        public void ManyProducersFewConsumersTest()
        {
            var _algorithm = new ProducerConsumerAlgorithm<int>(10, 2, () => 5, true);
            _algorithm.Run();
        }

        [Test]
        public void FewProducersManyConsumersTest()
        {
            var _algorithm = new ProducerConsumerAlgorithm<int>(2, 10, () => 5, true);
            _algorithm.Run();
        }
    }
}