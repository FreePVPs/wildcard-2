#include <iostream>
#include <vector>
#include <string>

using namespace std;

const int asize = 128;

int n, m, p;
int alph[asize];

struct BigInteger
{
    vector<int> digits;
    BigInteger()
    {
        this->digits.push_back(0);
    };
    BigInteger(int a)
    {
        this->digits.push_back(a);
    }
    BigInteger& operator+(BigInteger a)
    {
//        a.print();
//        cout << '+';
//        this->print();
//        cout << '=';
        BigInteger *ans = new BigInteger();
        ans->digits.pop_back();
        int carry = 0;
        for(int i=0;;i++)
        {
            if(int(this->digits.size()) > i)
            {
                carry += this->digits[i];
            }
            if(int(a.digits.size()) > i)
            {
                carry += a.digits[i];
            }
            ans->digits.push_back(carry);
            if(i >= (int)max(a.digits.size(), this->digits.size()))
                if(!ans->digits[ans->digits.size() - 1] && ans->digits.size() > 1)
                {
                    ans->digits.pop_back();
                    break;
                }
            carry = ans->digits[ans->digits.size() - 1] / 1000;
            ans->digits[ans->digits.size() - 1] %= 1000;
        }
//        ans->print();
//        cout << '\n';
        return *ans;
    }
    void print()
    {
        cout << this->digits[int(this->digits.size())-1];
        for(int i=int(this->digits.size())-2;i>=0;i--)
        {
            int a = digits[i];
            for(int j=0;j<3;j++)
            {
                cout << a / 100;
                a *= 10;
                a %= 1000;
            }
        }
    }
};


struct node
{
    int next[asize], go[asize];
    BigInteger *dyn[asize];
    int link, par, chr, calc = -1;
    bool term;
};

node tree[asize * 10];
int sz = 1;

void addstr(string s)
{
    int cur = 0;
    for(int i=0;i<(int)s.length();i++)
    {
        if(tree[cur].next[alph[(int)s[i]]] >= 0)
        {
            cur = tree[cur].next[alph[(int)s[i]]];
        }
        else
        {
            tree[cur].next[alph[(int)s[i]]] = sz;
            tree[sz].par = cur;
            cur = sz++;
            tree[cur].link = -1;
            tree[cur].chr = alph[(int)s[i]];
            for(int i=0;i<asize;i++)
            {
                tree[cur].go[i] = tree[cur].next[i] = -1;
            }
        }
    }
    tree[cur].term = 1;
}

int getgo(int v, int c);

int getlink (int v)
{
    if (tree[v].link >= 0)
    {
        return tree[v].link;
    }
    tree[v].link = (v && tree[v].par) ? getgo(getlink(tree[v].par), tree[v].chr) : 0;
    if(tree[tree[v].link].term)
    {
        tree[v].term = 1;
    }
    return tree[v].link;
}

int getgo(int v, int c)
{
    if (tree[v].go[c] >= 0)
    {
        return tree[v].go[c];
    }
    return tree[v].go[c] = (tree[v].next[c] >= 0) ? tree[v].next[c] : (v ? getgo(getlink(v), c) : 0);
}

void getdyn(int v, int c)
{
    if(tree[v].term || tree[v].calc == c)
    {
        return;
    }
    tree[v].calc = c;
    for(int i=0;i<n;i++)
    {
        int u = getgo(v, i);
        if(u >= 0)
        {
            if(!tree[u].dyn[c + 1])
                tree[u].dyn[c + 1] = new BigInteger();
            if(!tree[v].dyn[c])
            {
                tree[v].dyn[c] = new BigInteger();
            }
            *(tree[u].dyn[c + 1]) = *(tree[u].dyn[c + 1]) + *(tree[v].dyn[c]);
            getdyn(u, c);
        }
    }
}

const BigInteger &sum(int v, int c)
{
    if(tree[v].term || tree[v].calc == c + 1)
    {
        return *(new BigInteger());
    }
    tree[v].calc = c + 1;
    BigInteger *ans = new BigInteger();
    if(!tree[v].dyn[c])
    {
        tree[v].dyn[c] = new BigInteger();
    }
    *ans = *ans + *tree[v].dyn[c];
    for(int i=0;i<n;i++)
    {
        int u = getgo(v, i);
        if(u >= 0)
        {
            *ans = *ans + sum(u, c);
        }
    }
    return *ans;
}

int main()
{
    cin >> n >> m >> p;
    for(int i=0;i<n;i++)
    {
        char c;
        cin >> c;
        alph[(int)c] = i;
    }
    tree[0].link = -1;
    for(int i=0;i<asize;i++)
    {
        tree[0].next[i] = tree[0].go[i] = -1;
    }
    for(int i=0;i<p;i++)
    {
        string s;
        cin >> s;
        addstr(s);
    }
    tree[0].dyn[0] = new BigInteger(1);
    for(int i=0;i<m;i++)
    {
        getdyn(0, i);
    }
    BigInteger ans = sum(0, m);
    ans.print();
    return 0;
}
