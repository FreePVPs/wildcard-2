#include <cmath>
#include <cassert>
#include <algorithm>
#include <vector>
#include <iostream>
#include <cstdio>

#define DEVYATNADCAT 19



//SOSISKA!



using namespace std;

#define forn(i, n) for (int i = 0; i < (int)(n); i++)
#define sz(a) (int)(a).size()
#define LLINF 4e18

typedef double dbl;
typedef long long ll;

struct pnt
{
  int x, y;

  pnt( int _x = 0, int _y = 0 ) : x(_x), y(_y) { }

  inline pnt operator + ( const pnt &p ) const { return pnt(x + p.x, y + p.y); }
  inline pnt operator - ( const pnt &p ) const { return pnt(x - p.x, y - p.y); }

  inline ll operator * ( const pnt &p ) const { return (ll)x * p.y - (ll)y * p.x; }

  inline bool operator < ( const pnt &p ) const { return x < p.x || (x == p.x && y < p.y); }

  inline ll d2() const { return (ll)x * x + (ll)y * y; }
  inline dbl ang() const { return atan2((dbl)y, (dbl)x); }
};

typedef vector <pnt> vpnt;

inline bool pless( const pnt &a, const pnt &b )
{
  ll x = a * b;
  return x != 0 ? x < 0 : a.d2() < b.d2();
}

vpnt ConvexHull( vpnt p )
{
  int n = sz(p), mi = 0;
  assert(n > 0);
  forn(i, n)
    if (p[mi] < p[i])
      mi = i;
  swap(p[0], p[mi]);
  for (int i = 1; i < n; i++)
    p[i] = p[i] - p[0];
  sort(p.begin() + 1, p.end(), pless);

  int rn = 0;
  vpnt r(n);
  r[rn++] = p[0];
  for (int i = 1; i < n; i++)
  {
    pnt q = p[i] + p[0];
    while (rn >= 2 && (r[rn - 1] - r[rn - 2]) * (q - r[rn - 2]) >= 0)
      rn--;
    r[rn++] = q;
  }
  r.resize(rn);
  return r;
}

struct MagicStructure
{
  int N;
  pnt st;
  vector <pnt> p;
  vector <dbl> ang;

  void Build( int n, int *x, int *y ) // O(NlogN)
  {
    assert(n > 0);
    p.resize(N = n);
    forn(i, N)
      p[i].x = x[i], p[i].y = y[i];

    p = ConvexHull(p);
    N = sz(p);
    reverse(p.begin(), p.end());

    ang.resize(N);
    forn(i, N)
      ang[i] = (p[(i + 1) % N] - p[i]).ang();
    forn(i, N)
      if (i && ang[i] < ang[i - 1])
        ang[i] += 2 * M_PI;
  }

  ll GetMax( int a, int b ) // O(logK)
  {
    if (N < 3)
    {
      ll ma = -(ll)8e18;
      forn(l, N)
        ma = max(ma, (ll)a * p[l].x + (ll)b * p[l].y);
      return ma;
    }

    int l = 0, r = N - 1;
    dbl x = atan2(a, -b);
    while (x < ang[0])
      x += 2 * M_PI;
    while (l != r)
    {
      int m = (l + r + 1) / 2;
      if (ang[m] < x)
        l = m;
      else
        r = m - 1;
    }
    l = (l + 1) % N;
    return (ll)a * p[l].x + (ll)b * p[l].y;
  }
};

#undef forn
#undef sz

vector<int> xarr, yarr;

MagicStructure arr[DEVYATNADCAT * 2];

int main()
{
    freopen("mostfar.in", "r", stdin);
    freopen("mostfar.out", "w", stdout);
    int n, m;
    cin >> n;
    for(int i=0;i<n;i++)
    {
        int x, y;
        cin >> x >> y;
        xarr.push_back(x);
        yarr.push_back(y);
    }
    int c = 0;
    for(int i=DEVYATNADCAT;i>=0;i--)
    {
        if((1 << i) & n)
        {
            arr[i].Build(1 << i, &xarr[c], &yarr[c]);
            c += (1 << i);
        }
    }
    cin >> m;
    for(;m;m--)
    {
        char s[4];
        int a, b;
        cin >> s >> a >> b;
        if(s[0] == 'g')
        {
            long long ans = -LLINF;
            for(int i=0;i<=DEVYATNADCAT;i++)
            {
                ans = max(ans, arr[i].GetMax(a, b));
            }
            cout << ans << '\n';
        }
        else
        {
            xarr.clear();
            yarr.clear();
            xarr.push_back(a);
            yarr.push_back(b);
            for(int i=0;;i++)
            {
                if(arr[i].N)
                {
                    for(int j=0;j<arr[i].N;j++)
                    {
                        xarr.push_back(arr[i].p[j].x);
                        yarr.push_back(arr[i].p[j].y);
                    }
                    arr[i].p.clear();
                    arr[i].ang.clear();
                    arr[i].N = 0;
                }
                else
                {
                    arr[i].Build(1 << i, &xarr[0], &yarr[0]);
                    break;
                }
            }
        }
    }
    return 0;
}
