namespace Aelena.CommonExtensions;

/// <summary>Edit-distance metrics: an integer count of edits, where lower means closer.</summary>
public enum StringDistance
{
    /// <summary>Insertions, deletions and substitutions. See <see cref="StringDistanceExtensions"/>.</summary>
    Levenshtein,

    /// <summary>Levenshtein plus adjacent transpositions counted as one edit (optimal string alignment).</summary>
    DamerauLevenshtein,
}

/// <summary>Similarity metrics: a score from 0 (nothing in common) to 1 (identical), where higher means closer.</summary>
public enum StringSimilarity
{
    /// <summary>Jaro similarity with Winkler's bonus for a shared prefix. Best for names and short identifiers.</summary>
    JaroWinkler,

    /// <summary>Jaro similarity without the prefix bonus.</summary>
    Jaro,

    /// <summary>1 minus the Levenshtein distance divided by the longer length.</summary>
    Levenshtein,

    /// <summary>Twice the longest common subsequence divided by the total length.</summary>
    LongestCommonSubsequence,

    /// <summary>Sørensen-Dice coefficient over the sets of character bigrams.</summary>
    Dice,

    /// <summary>Jaccard index over the sets of character bigrams.</summary>
    Jaccard,
}
