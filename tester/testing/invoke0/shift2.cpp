#include <bits/stdc++.h>

using namespace std;

typedef long long ll;
typedef pair<int, int> pii;

#define fi first
#define se second
#define mp make_pair

char s[20000005];

vector<int> v;

int main()
{
    ll n, q, x1, x2, x3;
    cin >> n >> q >> x1 >> x2;
    for (int i = 0; i < n; i++)
    {
        x3 = (534517LL*x1+151469LL*x2+769478543LL) % (1LL << 32);
        x1 = x2;
        x2 = x3;
        s[i] = 'a' + (x2%q);
    }
    for (int i = 0; i < n; i++) s[i+n] = s[i];
    n *= 2;
    n++;
    int i = 0, k = 0, j = 1;
    while(i < n)
    {
        j = i+1;
        k = i;
        while (j < n && s[k] <= s[j])
        {
            if (s[j] == s[k])
            {
                j++;
                k++;
            }
            else if(s[j] > s[k])
            {
                j++;
                k = i;
            }
        }
        while (i <= k)
        {
            v.push_back(i);
            i += j-k;
        }
    }
    n /= 2;
    int res = 0;
    for (size_t i = 0; i < v.size(); i++)
    {
        if (v[i] >= n) break;
        res = v[i];
    }
    cout << res+1;
    return 0;
}
