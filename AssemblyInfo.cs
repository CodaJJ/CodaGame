using System.Runtime.CompilerServices;

// Allow Editor-only assemblies to access internal types of the CodaGame runtime assembly.
[assembly: InternalsVisibleTo("CodaGame.Tools.Editor")]
[assembly: InternalsVisibleTo("CodaGame.Tools.Attribute.Editor")]
[assembly: InternalsVisibleTo("CodaGame.Tools.Types.Editor")]
[assembly: InternalsVisibleTo("CodaGame.GameConfig.Editor")]
