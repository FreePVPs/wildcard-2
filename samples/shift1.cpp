#include <iostream>
#include <vector>
#include <algorithm>

using namespace std;

char s[20000005];
vector<int> cut;
int n, k;
unsigned long long x1, x2;

void gen()
{
    for(int i=0;i<n;i++)
    {
        x1 = (534517 * x1 + 151469 * x2 +  769478543);
        x1 = (unsigned) x1;
        swap(x1, x2);
        s[i] = s[n + i] = 'a' + (x2 % k);
    }
}

void duval()
{
    int i, j, k;
    for(i=0;i<n;)
    {
        j = i + 1;
        k = i;
        while(j < n && s[k] <= s[j])
        {
            if(s[k] < s[j]) //unite W
            {
                k = i;
            }
            else
            {
                k++;
            }
            j++;
        }
        while(i <= k)
        {
            cut.push_back(i);
            i += j - k;
        }
    }
}

int main()
{
    cin >> n >> k;
    cin >> x1 >> x2;
    gen();
    n *= 2;
    //cin >> s;
    duval();
    int lp = *(lower_bound(cut.begin(), cut.end(), n / 2) - 1);
    cout << lp + 1;
    return 0;
}
