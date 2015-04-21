#include <bits/stdc++.h>

using namespace std;

typedef long long ll;

struct treap
{
    int l, r;
    int a, sz;
    ll sum;
    treap()
    {
        l = r = -1;
        sz = 1;
    }
    treap (int a):a(a)
    {
        l = r = -1;
        sz = 1;
        sum = a;
    }
    treap (treap &a)
    {
        l = a.l;
        r = a.r;
        this->a = a.a;
        sum = a.sum;
        sz = a.sz;
    }
};

const int MAXSZ = 10000000;

treap A[2*MAXSZ];
int na=0;

int size(int a)
{
    if (a == -1) return 0;
    return A[a].sz;
}

ll sum(int a)
{
    if (a == -1) return 0;
    return A[a].sum;
}

void recalc(int T)
{
    if (T == -1) return;
    A[T].sum = sum(A[T].l) + sum(A[T].r) + A[T].a;
    A[T].sz = size(A[T].l) + size(A[T].r) + 1;
}

int newtreap (int a)
{
    if (a == -1) return -1;
    A[na++] = A[a];
    return na-1;
}

void merge(int &T, int l, int r)
{
    //l = newtreap(l);
    //r = newtreap(r);
    if (l == -1)
    {
        T = r;
        recalc(T);
        return;
    }
    if (r == -1)
    {
        T = l;
        recalc(T);
        return;
    }
    int q = rand()%(size(l)+size(r));
    if (q < size(l))
    {
        l = newtreap(l);
        T = l;
        //T->r = newtreap(T->r);
        merge(A[T].r, A[T].r, r);
    }
    else
    {
        r = newtreap(r);
        T = r;
        //T->l = newtreap(T->l);
        merge(A[T].l, l, A[T].l);
    }
    recalc(T);
}

void smerge(int &T, int l, int r)
{
    //l = newtreap(l);
    //r = newtreap(r);
    if (l == -1)
    {
        T = r;
        recalc(T);
        return;
    }
    if (r == -1)
    {
        T = l;
        recalc(T);
        return;
    }
    int q = rand()%(size(l)+size(r));
    if (q < size(l))
    {
        //l = newtreap(l);
        T = l;
        //T->r = newtreap(T->r);
        smerge(A[T].r, A[T].r, r);
    }
    else
    {
        //r = newtreap(r);
        T = r;
        //T->l = newtreap(T->l);
        smerge(A[T].l, l, A[T].l);
    }
    recalc(T);
}

void split(int T, int &l, int &r, int x)
{
    if (T == -1)
    {
        l = r = -1;
        return;
    }
    int tt = T;
    T = na++;
    A[T] = treap(A[tt]);
    //T->pts++;
    if (size(A[T].l) >= x)
    {
        r = T;
        split(A[r].l, l, A[r].l, x);
    }
    else
    {
        l = T;
        split(A[l].r, A[l].r, r, x-size(A[T].l)-1);
    }
    recalc(l);
    recalc(r);
}

void printt (int a)
{
    if (a == -1) return;
    printt(A[a].l);
    cout << A[a].a << " ";
    printt(A[a].r);
}

int tb[MAXSZ];

void build(int &T, int l, int r)
{
    if (l >= r) return;
    int m = (l+r)/2;
    //T = new treap(tb[m]);
    T = na++;
    A[na-1] = treap(tb[m]);
    if (l+1 == r) return;
    build(A[T].l, l, m);
    build(A[T].r, m+1, r);
    recalc(T);
}

int exps;

void exp(int a)
{
    if (a == -1) return;
    exp(A[a].l);
    tb[exps++] = A[a].a;
    exp(A[a].r);
}

void check(int& T)
{
    if (na <= MAXSZ) return;
    exps = 0;
    exp(T);
    int Ts = size(T);
    na = 0;
    build(T, 0, Ts);
}

int main()
{
    ios_base::sync_with_stdio(0);
    srand(137);
    int T = -1;
    ll n, x, a, b, m;
    cin >> n >> x >> a >> b >> m;
    //pt qT = T;
    //merge(T, qT, shared_ptr<treap>(new treap(x)));
    tb[0] = x;
    //printt(T);
    //cout << endl;
    for (int i = 1; i < n; i++)
    {
        x = (a*x+b)%m;
        tb[i] = x;
        //qT = T;
        //smerge(T, qT, shared_ptr<treap>(new treap(x)));
        //printt(T);
        //cout << endl;
    }
    build(T, 0, n);
    int k;
    cin >> k;
    string s;
    int l, r;
    //printt(T);
    //cout << endl;
    for (int i = 0; i < k; i++)
    {
        cin >> s;
        if (s == "sum")
        {
            cin >> l >> r;
            l--;
            r--;
            int q = -1,w = -1;
            split(T, q, w, l);
            split(w, q, w, r-l+1);
            cout << sum(q) << endl;
            check(T);
        }
        else if (s == "cpy")
        {
            int q = -1,w = -1,e = -1,Q = -1,W = -1,E = -1;
            cin >> a >> b >> l;
            a--;
            b--;
            split(T, q, w, a);
            split(w, e, q, l);
            split(T, Q, W, b);
            split(W, W, E, l);
            merge(T, Q, e);
            merge(T, T, E);
            check(T);
        }
        else
        {
            cin >> l >> r;
            int q = -1, w = -1;
            split(T, q, w, l-1);
            split(w, q, w, r-l+1);
            printt(q);
            check(T);
            cout << endl;
        }
    }
    return 0;
}
