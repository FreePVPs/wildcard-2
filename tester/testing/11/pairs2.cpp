#include <bits/stdc++.h>

using namespace std;

const int maxn = 500;

vector<int> g[maxn];
bool used[maxn];
int t[maxn];

bool dfs(int v)
{
    if (used[v]) return 0;
    used[v] = 1;
    for (size_t i = 0; i < g[v].size(); i++)
    {
        int u = g[v][i];
        if (t[u] == -1 || dfs(t[u]))
        {
            t[u] = v;
            return 1;
        }
    }
    return 0;
}

int main()
{
    int n, m;
    cin >> n >> m;
    for (int i = 0; i < m; i++) t[i] = -1;
    for (int i = 0; i < n; i++)
    {
        int q;
        while(1)
        {
            cin >> q;
            if (q == 0) break;
            g[i].push_back(q-1);
        }
    }
    for (int i = 0; i < n; i++)
    {
        for (int j = 0; j < n; j++) used[j] = 0;
        dfs(i);
    }
    int r = 0;
    for (int i = 0; i < m; i++)
    {
        if (t[i] != -1)
        {
            r++;
        }
    }
    cout << r << endl;
    for (int i = 0; i < m; i++)
    {
        if (t[i] != -1)
        {
            cout << t[i]+1 << " " << i+1 << endl;
        }
    }
    return 0;
}
