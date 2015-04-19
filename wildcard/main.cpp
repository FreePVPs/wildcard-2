#include <bits/stdc++.h>

using namespace std;

const double alpha = 1.6;

int arr[2000][2000];

int dsu[128];

bool inComment;

int dsu_root(int v)
{
    if(v == dsu[v])
        return v;
    return dsu[v] = dsu_root(dsu[v]);
}

void dsu_merge(int a, int b)
{
    a = dsu_root(a);
    b = dsu_root(b);
    dsu[a] = b;
}

bool compare(vector<long long> &f1, vector<long long> &f2)
{
    for(int i=1;i<max(f1.size(), f2.size());i++)
    {
        arr[0][i] = arr[i][0] = i;
    }
    for(int i=1;i<=f1.size();i++)
    {
        for(int j=1;j<=f2.size();j++)
        {
              arr[i][j] = min(min(arr[i][j-1], arr[i-1][j]) + 1, (arr[i-1][j-1] + (f1[i] != f2[j])));
        }
    }
    long long ans = arr[f1.size()][f2.size()];
    if(ans * alpha < min(f1.size(), f2.size()))
        return 1;
    return 0;
}

inline bool isLetter(char a)
{
    return ('a' <= a && a <= 'z') || ('A' <= a && a <= 'Z');
}

inline bool isDigit(char a)
{
    return ('0' <= a && a <= '9');
}

long long  phash(string s)
{
    long long ans = 0;
    for(int i=0;i<s.length();i++)
    {
        if(s[i] == ' ' || s[i] == '\t' || s[i] == '\n' || s[i] == '\r')
            continue;
        if(i < s.length() - 1 && s[i] == '/' && s[i+1] == '/')
            break;
        if(i < s.length() - 1 && s[i] == '/' && s[i+1] == '*')
            inComment = 1;
        if(i < s.length() - 1 && s[i] == '*' && s[i+1] == '/')
            inComment = 0;
        if(s[i] == '#')
            break;
        if(inComment)
            continue;
        if(isLetter(s[i]))
        {
            ans *= 257;
            ans += '0';
            continue;
        }
        if(isDigit(s[i]))
        {
            ans *= 257;
            ans += '0';
            continue;
        }
        ans *= 257;
        ans += s[i];
    }
    return ans;
}

int main()
{
    ios_base::sync_with_stdio(0);
    freopen("input.txt", "r", stdin);
    freopen("output.txt", "w", stdout);
    int n;
    cin >> n;
    vector<string> files;
    files.resize(n);
    for(int i=0;i<n;i++)
    {
        cin >> files[i];
        dsu[i] = i;
    }
    vector<long long> file1, file2;
    for(int i=0;i<n-1;i++)
    {
        for(int j=1;j<n;j++)
        {
            fstream f1(files[i]);
            fstream f2(files[j]);
            file1.clear();
            file2.clear();
            string buf;
            inComment = 0;
            while(!f1.eof())
            {
                getline(f1, buf);
                long long t = phash(buf);
                if(t)
                    file1.push_back(t);
            }
            inComment = 0;
            while(!f2.eof())
            {
                getline(f2, buf);
                long long t = phash(buf);
                if(t)
                    file2.push_back(t);
            }
            f1.close();
            f2.close();
            if(compare(file1, file2))
                dsu_merge(i, j);
        }
    }
    vector<int> ans[128];
    for(int i=0;i<n;i++)
    {
        ans[dsu_root(i)].push_back(i);
    }
    int ansc = 0;
    for(int i=0;i<n;i++)
    {
        if(ans[i].size() >= 2)
            ansc++;
    }
    cout << ansc << endl;
    for(int i=0;i<n;i++)
    {
        if(ans[i].size() < 2)
            continue;
        for(int j=0;j<ans[i].size();j++)
        {
            cout << files[ans[i][j]] << ' ';
        }
        cout << endl;
    }
    return 0;
}
