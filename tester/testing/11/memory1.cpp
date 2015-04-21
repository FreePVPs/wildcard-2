#include <cstdio>
#include <algorithm>

using namespace std;

const int sz = 1e6;

struct treap
{
    treap *l, *r;
    int x, sz;
    long long sum;
    treap(treap &t)
    {
        this->l = t.l;
        this->r = t.r;
        this->x = t.x;
        this->sz = t.sz;
        this->sum = t.sum;
    }
    treap(){};
};

typedef treap* ptreap;

int arr[sz];
char mem[400 * sz];
int memptr = 0;
int arrptr;
ptreap root;
int n, m;

void* operator new(size_t a)
{
    memptr += a;
    return mem + memptr - a;
}

inline int random(int n)
{
    return rand() % n;
}

inline int getsize(ptreap p)
{
    return p?p->sz:0;
}

inline long long getsum(ptreap p)
{
    return p?p->sum:0;
}

void recalc(ptreap p)
{
    if(p)
    {
        p->sum = getsum(p->l) + getsum(p->r) + p->x;
        p->sz = getsize(p->l) + getsize(p->r) + 1;
    }
}

void printtree(ptreap t)
{
    if(!t)
    {
        return;
    }
    printtree(t->l);
    printf("%d ", t->x);
    printtree(t->r);
}

void merge(ptreap &T, ptreap pl, ptreap pr)
{
    if(!pl)
    {
        T = pr;
        recalc(T);
        return;
    }
    if(!pr)
    {
        T = pl;
        recalc(T);
        return;
    }
    int ls = pl->sz, rs = pr->sz;
    if(random(ls + rs) >= ls)
    {
        T = new treap(*pr);
        merge(T->l, pl, T->l);
    }
    else
    {
        T = new treap(*pl);
        merge(T->r, T->r, pr);
    }
    recalc(T);
}

void split(ptreap T, int x, ptreap &pl, ptreap &pr)
{
    if(!T)
    {
        pl = T;
        pr = T;
        return;
    }
    T = new treap(*T);
    int sz = getsize(T->l);
    if(x > sz)
    {
        pl = T;
        split(pl->r, x - sz - 1, pl->r, pr);
    }
    else
    {
        pr = T;
        split(pr->l, x, pl, pr->l);
    }
    recalc(pl);
    recalc(pr);
}

void buildtree(ptreap &T, int l, int r)
{
    if(l >= r)
    {
        return;
    }
    int m = (l + r) / 2;
    T = new treap();
    T->l = 0;
    T->r = 0;
    T->x = T->sum = arr[m];
    T->sz = 1;
    if(l + 1 < r)
    {
        buildtree(T->l, l, m);
        buildtree(T->r, m + 1, r);
        recalc(T);
    }
}

void buildarr(ptreap T)
{
    if(!T)
    {
        return;
    }
    buildarr(T->l);
    arr[arrptr++] = T->x;
    buildarr(T->r);
}

int main()
{
    scanf("%d", &n);
    long long a, b, mod;
    scanf("%d %lld %lld %lld", arr, &a, &b, &mod);
    for(int i=1;i<n;i++)
    {
        arr[i] = (a * arr[i - 1] + b) % mod;
    }
    scanf("%d", &m);
    buildtree(root, 0, n);
    for(int i=0;i<m;i++)
    {
        char cmd[4];
        int x, y, z;
        scanf(" %s %d %d", cmd, &x, &y);
        x--;
        y--;
        if(cmd[0] == 'c')      //cpy
        {
            scanf("%d", &z);
            ptreap p1, p2, p3, p4, p5, p6, p7, p8;
            split(root, x, p1, p2);
            split(p2, z, p3, p4);
            split(root, y, p5, p6);
            split(p6, z, p7, p8);
            merge(root, p5, p3);
            merge(root, root, p8);
        }
        else if(cmd[0] == 's') //sum
        {
            ptreap p1, p2, p3, p4;
            split(root, x, p1, p2);
            split(p2, y - x + 1, p3, p4);
            printf("%lld\n", getsum(p3));
        }
        else                   //out
        {
            ptreap p1, p2, p3, p4;
            split(root, x, p1, p2);
            split(p2, y - x + 1, p3, p4);
            printtree(p3);
            printf("\n");
        }
        if(memptr > 390 * sz)
        {
            arrptr = 0;
            buildarr(root);
            memptr = 0;
            buildtree(root, 0, n);
        }
    }
    return 0;
}
