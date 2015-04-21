import java.math.BigInteger;
import java.util.*;

public class Main {

    public static void main(String[] args) {
        Main main = new Main();
    }
    
    int maxn = 200;
    
    class Auto
    {
        public int pch;
        public int alph;
        public BigInteger d[] = new BigInteger[maxn];
        public Auto next[] = new Auto[maxn];
        public boolean next_calced[] = new boolean[maxn];
        public Auto link;
        public Auto p;
        public int tk = -1;
        public boolean term = false;
        
        public Auto()
        {
            for (int i = 0; i < maxn; i++)
            {
                d[i] = BigInteger.ZERO; 
                next_calced[i] = false;
            }
        }
        
        Auto go(int v)
        {
            if (next_calced[v]) return next[v];
            next_calced[v] = true;
            if (next[v] != null) return next[v];
            if (p == null)
            {
                next[v] = this;
                return this;
            }
            //else if (p.p == null)
            //{
            //    a[v] = p;
            //    return a[v];
            //}
            next[v] = link.go(v);
            return next[v];
        }
        
        void calcd(int k)
        {
            if (term) return;
            if (tk == k) return;
            tk = k;
            for (int i = 0; i < alph; i++)
            {
                Auto u = go(i);
                if (u == null) continue;
                u.d[k+1] = u.d[k+1].add(d[k]);
                u.calcd(k);
            }
        }
        
        BigInteger sum(int k)
        {
            if (tk == k+1 || term) return BigInteger.ZERO;
            tk = k+1;
            BigInteger r = BigInteger.ZERO;
            r = r.add(d[k]);
            for (int i = 0; i < alph; i++)
            {
                Auto u = go(i);
                if (u == null) continue;
                r = r.add(u.sum(k));
            }
            return r;
        }
        
    }

    String[] s = new String[10];
    
    public Main() {
        Scanner sc = new Scanner(System.in);
        int n, m, p;
        n = sc.nextInt();
        m = sc.nextInt();
        p = sc.nextInt();
        String sa = sc.nextLine();
        sa = sc.nextLine();
        TreeMap<Character, Integer> map = new TreeMap();
        for(int i = 0; i < sa.length(); i++)
        {
            map.put(sa.charAt(i), i);
        }
        for (int i = 0; i < p; i++)
            s[i] = sc.nextLine();
        Auto root = new Auto();
        root.link = root;
        root.alph = sa.length();
        for (int i = 0; i < p; i++)
        {
            Auto t = root;
            for (int j = 0; j < s[i].length(); j++)
            {
                int g = map.get(s[i].charAt(j));
                if (t.next[g] != null)
                {
                    t = t.next[g];
                }
                else 
                {
                    t.next[g] = new Auto();
                    t.next[g].p = t;
                    t.next[g].alph = sa.length();
                    t.next[g].pch = g;
                    t = t.next[g];
                }
            }
            t.term = true;
        }
        ArrayDeque<Auto> q = new ArrayDeque<>();
        q.push(root);
        while (!q.isEmpty())
        {
            Auto v = q.getFirst();
            q.pop();
            if (v.p != null)
            {
                if (v.p.p != null)
                {
                    v.link = v.p.link.go(v.pch);
                }
                else
                {
                    v.link = v.p;
                }
            }
            if (v.link.term) v.term = true;
            for (int i = 0; i < sa.length(); i++)
            {
                Auto u = v.next[i];
                if (u == null) continue;
                q.add(u);
            }
        }
        root.d[0] = BigInteger.ONE;
        for (int i = 0; i < m; i++)
        {
            root.calcd(i);
        }
        System.out.println(root.sum(m));
    }
    
}
