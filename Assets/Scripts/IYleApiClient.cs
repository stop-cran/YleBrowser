using System.Collections;
using System.Collections.Generic;

public interface IYleApiClient
{
    bool Success { get; }
    string Error { get; }
    IReadOnlyList<ItemModel> Result { get; }

    IEnumerator Search(string query, int offset, int limit);
}
