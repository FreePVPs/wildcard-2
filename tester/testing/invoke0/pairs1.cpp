#include <iostream>
#include <vector>

using namespace std;

vector<int> graph[1000];
int n, m;
bool used[1000];
int mt[1000];

bool dfs(int v)
{
    if(used[v])
    {
        return 0;
    }
    used[v] = 1;
    for (int i=0;i<(int)graph[v].size();i++)
    {
        int t = graph[v][i];
        if (mt[t] == -1 || dfs(mt[t]))
        {
            mt[t] = v;
            return 1;
        }
    }
    return 0;
}

int main()
{
    cin >> n >> m;
    for(int i=0;i<n;i++)
    {
        int a;
        cin >> a;
        while(a)
        {
            graph[i].push_back(a - 1);
            cin >> a;
        }
    }
    for(int i=0;i<m;i++)
    {
        mt[i] = -1;
    }
    for(int i=0;i<n;i++)
    {
        for(int j=0;j<n;j++)
        {
            used[j] = 0;
        }
        dfs(i);
    }
    int ans = 0;
    for (int i=0; i<m; i++)
    {
        ans += (mt[i] != -1);
    }
    cout << ans << '\n';
    for (int i=0; i<m; i++)
    {
        if (mt[i] != -1)
        {
            cout << mt[i] + 1 << ' ' << i + 1 << '\n';
        }
    }
    return 0;
}
