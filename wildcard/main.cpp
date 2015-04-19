#include <bits/stdc++.h>

using namespace std;

const double alpha1 = 1.6;
const double alpha2 = 1.15;

int arr[2000][2000];

int dsu[128];

bool inComment;
string curExt;

vector<long long> hashes[128];

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

string fileExt(string &f)
{
    int fp = f.find('.') + 1;
    return f.substr(fp, f.size() - fp);
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
    if(ans * alpha1 < min(f1.size(), f2.size()))
        return 1;
    int c = 0;
    for(int i=0;i<f1.size();i++)
    {
        if(find(f2.begin(), f2.end(), f1[i]) != f2.end())
        {
            c++;
        }
    }
    if(c * alpha2 > f1.size())
        return 1;
    c = 0;
    for(int i=0;i<f2.size();i++)
    {
        if(find(f1.begin(), f1.end(), f2[i]) != f1.end())
        {
            c++;
        }
    }
    if(c * alpha2 > f2.size())
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

long long phash(string s)
{
    long long ans = 0;
   /* if(curExt == "py")
    {
        if(s.substr(0, 6) == "import" || s.substr(0, 4) == "from")
            return 0;
    }
    if(curExt[0] == 'j')
    {
        if(s.substr(0, 6) == "import")
            return 0;
    }*/
    for(int i=0;i<s.length();i++)
    {
        if(s[i] == ' ' || s[i] == '\t' || s[i] == '\n' || s[i] == '\r')
            continue;
        if(curExt[0] == 'c' || curExt[0] == 'j') ///C-like
        {
            if(i < s.length() - 1 && s[i] == '/' && s[i+1] == '/')
                break;
            if(i < s.length() - 1 && s[i] == '/' && s[i+1] == '*')
                inComment = 1;
            if(i < s.length() - 1 && s[i] == '*' && s[i+1] == '/')
                inComment = 0;
            if(s[i] == '#')
                break;
        }
        if(curExt == "py")
        {
            if(s[i] == '#')
                break;
        }
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
        curExt = fileExt(files[i]);
        fstream f1(files[i]);
        string buf;
        inComment = 0;
        while(!f1.eof())
        {
            getline(f1, buf);
            long long t = phash(buf);
            if(t)
                hashes[i].push_back(t);
        }
        f1.close();
    }
    for(int i=0;i<n-1;i++)
    {
        for(int j=i+1;j<n;j++)
        {
            if(i == j)
                continue;
            if(fileExt(files[i]) != fileExt(files[j]))
                continue;
            if(compare(hashes[i], hashes[j]))
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
