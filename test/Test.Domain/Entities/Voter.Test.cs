// using System.ComponentModel;
// using Domain.Entities;
//
// namespace Test.Domain.Entities;
//
// [EditorBrowsable(EditorBrowsableState.Never)]
// internal class TestVoter
// {
//     private static readonly Voter Voter = Voter.NewVoter();
//
//     [Test]
//     [Parallelizable]
//     public void TestSign()
//     {
//         var payload = "aaa"u8.ToArray();
//         Assert.That(Voter.Sign(payload, out var signature), Is.True);
//         Assert.That(Voter.Verify(payload, signature!), Is.True);
//     }
//
//     [Test]
//     [Parallelizable]
//     public void TestTwoVotersHaveDifferentSignatures()
//     {
//         var payload = "aaa"u8.ToArray();
//         Assert.That(Voter.Sign(payload, out var signature), Is.True);
//
//         var voter = Voter.NewVoter();
//
//         Assert.That(Voter.Verify(payload, signature!), Is.True);
//         Assert.That(voter.Verify(payload, signature!), Is.False);
//     }
//
//     [Test]
//     [Parallelizable]
//     public void TestLoadedPublicKeyCanVerifySignature()
//     {
//         var payload = "aaa"u8.ToArray();
//         Assert.That(Voter.Sign(payload, out var signature), Is.True);
//
//         var voter = Voter.FromPubKey(Voter.PublicKey);
//
//         Assert.That(voter.Verify(payload, signature!), Is.True);
//     }
//
//     [Test]
//     [Parallelizable]
//     public void TestCannotSignWithoutSecretKey()
//     {
//         var voter = Voter.FromPubKey(Voter.PublicKey);
//         Assert.That(voter.Sign("aaa"u8.ToArray(), out _), Is.False);
//     }
//
//     [Test]
//     [Parallelizable]
//     public void TestAddressGetHashCode()
//     {
//         Assert.That(Voter.GetHashCode(), Is.EqualTo(Voter.Address.GetHashCode()));
//     }
// }