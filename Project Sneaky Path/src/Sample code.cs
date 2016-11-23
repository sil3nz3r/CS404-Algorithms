/*
  Declaring and Initializaing variables
*/

// (2)
n = 6;
this = '';
EdgeCost = int[n,n]

// (3)
brand = rand(2, 12)
big = 1000 * n^3;

// (4)
MyDist[0] =  int[n, n];
FirstStop[0] = int[n, n];
LastStop[0] = int[n, n];
PathTraffic = int[n];
EdgeTraffic = int[n];

k = 0
for (i from 1 to n)
{
  for (j from 1 to n)
  {

    MyDist[k][i,j] = brand() // + 3 * (abs(i = j) - 1)^2
    // This is the F-matrix for the flow of traffic
    PathTraffic[i,j] = n + brand();
    // This is the E-matrix for the traveling time
    EdgeCost[i,j] = MyDist [k][i,j];
    EdgeTraffic[i,j] = 0;
    FirstStop[k][i,j] = j;
    LastStop[k][i,j] = i;

    // If the distane is larger than 10, deem it big
    // This is probably for simplicity sake
    if (MyDist[k][i,j] > 10)
    {
      MyDist[k][i,j] = big;
      FirstStop[k][i,j] = -1;
      LastStop[k][i,j] = -1;
      EdgeCost[i,j] = na;
    }
    //MyDist[k][i,j] = (i - j)^2 * (brand())^2
  }
  // No circular link
  MyDist[k][i,i] = 0;
  FirstStop[k][i,i] = 0;
  LastStop[k][i,i] = 0;
  EdgeTraffic[i,i] = 0;
  PathTraffic[i,i] = 0
  EdgeCost[i,i] = '';
}

// (5)
print(k, EdgeCost, PathTraffic);

// (6)
for (k from 1 to n)
{
  // Set these matrices to max values
  MyDist[k] = int[n,n];
  FirstStop[k] = int[n,n];
  LastStop[k] = int[n,n];
  //print(k - 1, MyDist[k - 1], FirstStop[k - 1], LastStop[k - 1]);

  for (i from 1 to n)
  {
    for (j from 1 to n)
    {
      // First route: go from one note to the other
      optionDirect = MyDist[k - 1][i,j];
      // Second route: go to a intermediate node k
      optionViaK = MyDist[k - 1][i,k] + MyDist[k - 1][k, j];

      // Compare the two routes, take the smaller
      // or favors the direct option in case of tie
      if (optionDirect <= optionViaK)
      {
        // Direct route
        MyDist[k][i,j] = optionDirect;
        FirstStop[k][i,j] = FirstStop[k - 1][i,j];
        LastStop[k][i,j] = LastStop[k - 1][i,j];
        //lprint('Direct: then', optionDirect, optionViaK, k, i, j);
      }
      else
      {
        // intermediate route
        MyDist[k][i,j] = optionViaK;
        FirstStop[k][i,j] = FirstStop[k - 1][i,k];
        LastStop[k][i,j] = LastStop[k - 1][k,j];
        //lprint('via K: else', optionDirect, optionViaK, k, i, j);
      }
    } //j
  } //i
  //print(k, MyDist[k], FirstStop[k], LastStop[k]);
} //k

k = 0;
print(k, MyDist[k], FirstStop[k], LastStop[k]);
k = n;
print(k, MyDist[k], FirstStop[k], LastStop[k]);


// (7)
printlevel = 1;
evalm(FirstStop[n]);


// (8)
Hops = int(n);
ActualPath = int(n);

for (iii from 1 to n)
{
  for (jjj from 1 to n)
  {
    ActualPath[iii,jjj] = iii;
  }
}

for (ii from 1 to n)
{
  for (jj from 1 to n)
  {
    i = ii;
    j = jj;

    while (FirstStop[n][i,j] != 0)
    {
      ifrom = i;
      i = FirstStop[n][i,j];
      Hops[ii,jj] = Hops[ii,jj] + 1;
      EdgeTraffic[ifrom, i] = EdgeTraffic[ifrom,i] + PathTraffic[ii, jj];
      ActualPath[ii,jj] = ActualPath[ii,jj] + i // maybe???
    }
    // print('For ', ii, jj, ' the actual path is ', ActualPath[ii,jj]);
  }
}
print(EdgeCost, MyDist[n], ActualPath, Hops, PathTraffic, EdgeTraffic);


// (9)
FlowPerEdge[0] = int[n,n];
FirstNewStop[0] = int[n,n];
LastNewStop[0] = int[n,n];
whattype(FlowPerEdge[0]);


// (10)
for (i from 1 to n)
{
  for (j from 1 to n)
  {
    if (MyDist[0][i,j] == big)
    {
      FlowPerEdge[0][i,j] = big;
      EdgeTraffic[i,j] = na;
    }
    else
    {
      FlowPerEdge[0][i,j] = max(0, EdgeTraffic[i,j]);
    }
  }
  FlowPerEdge[0][i,j] = 0;
}
whattype(FlowPerEdge[0]);

print(FlowPerEdge[0], Hops, ActualPath, PathTraffic, EdgeTraffic);


// (11)
FirstNewStop[0] = int[n,n];
LastNewStop = int[n,n];

k = 0;
for (i from 1 to n)
{
  for (j from 1 to n)
  {
    FirstNewStop[k][i,j] = j;
    LastNewStop[k][i,j] = i;
  }
  FirstNewStop[k][i,i] = 0;
  LastNewStop[k][i,i] = 0;
}


// (12)
print(FlowPerEdge[0], Hops, ActualPath, PathTraffic, EdgeTraffic);


// (13)
for (k from 1 to n)
{
  FlowPerEdge[k] = int[n,n];
  FirstNewStop[k] = int[n,n];
  LastNewStop[k] = int[n,n];
  //print(k - 1, FlowPerEdge[k - 1], FirstNewStop[k - 1], LastNewStop[k - 1]);

  for (i from 1 to n)
  {
    for (j from 1 to n)
    {
      optionDirect = FlowPerEdge[k - 1][i, j];
      //optionViaK = max(FlowPerEdge[k - 1][i,k], FlowPerEdge[k - 1][k, j]);
      optionViaK = FlowPerEdge[k - 1][i,k] + FlowPerEdge[k - 1][k, j];

      if (optionDirect <= optionViaK)
      {
        // direct rout
        FlowPerEdge[k][i,j] = optionDirect;
        FirstNewStop[k][i,j] = FirstNewStop[k - 1][i,j];
        LastNewStop[k][i,j] = LastNewStop[k - 1][i, j];
        //lprint('Direct: then ', optionDirect. optionViaK, k, i, j);
      }
      else
      {
        // intermediate route
        FlowPerEdge[k][i,j] = optionViaK;
        FirstNewStop[k][i,j] = FirstStop[k - 1][i,k];
        LastNewStop[k][i,j] = LastStop[k - 1][k,j];
        //lprint('via K: else', optionDirect, optionViaK, k, i, j);
      }
    } // j
  } // i
  //print(k, FlowPerEdge[k], FirstNewStop[k], LastNewStop[k]);
} // k
k = 0;
print(k, FlowPerEdge[k], FirstNewStop[k], LastNewStop[k]);

// (14)
k = n;
print(k, FlowPerEdge[k], FirstNewStop[k], LastNewStop[k]);


// (15)
Hops2 = int[n];
ActualPath2 = int[n];

for (iii from 1 to n)
{
  for (jjj from 1 to n)
  {
    ActualPath2[iii, jjj] = iii;
  }
}
MinSneakyPath = int[n];
MaxSneakyPath = int[n];
AvgSneakyPath = int[n];

for (ii from 1 to n)
{
  for (jj from 1 to n)
  {
    MinSneakyPath[ii, jj] = big;
    MaxSneakyPath[ii, jj] = 0;
    SumSneakyPath[ii, jj] = 0; // shouldn't this be AvgSneakyPath = int[n]???

    while (FirstStop[n][i, j] != 0)
    {
      ifrom = i;
      i = FirstNewStop[n][i, j];
      Hops2[ii, jj] = Hops2[ii, jj] + 1;

      MinSneakyPath[ifrom, i] = min(MinSneakyPath[ii, jj], FlowPerEdge[k][ifrom, i]);
      MaxSneakyPath[ii, jj] = max(MinSneakyPath[ii, jj], FlowPerEdge[k][ifrom, i]);
      AvgSneakyPath[ii, jj] = AvgSneakyPath[ii, jj] + FlowPerEdge[k][ifrom, i];
      //TotalFlow[ifrom, i] = TotalFlow[ifrom, i] + PathTraffic[ii, jj];
      ActualPath2[ii, jj] = ActualPath2[ii, jj], i; // maybe ActualPath2[ii, jj] + i
    }

    if (Hops2[ii, jj] > 0
    {
      AvgSneakyPath[ii, jj] = AvgSneakyPath[ii, jj] / Hops2[ii, jj];
    }

    //print('For ', ii, jj, ' the actual path is ', ActualPath2[ii, jj]);
  }
}

for (iii from 1 to n)
{
  MinSneakyPath[iii, iii] = 0
}

print(evalm(Hops), evalm(Hops), evalm(Hops - Hops2));
print(ActualPath, ActualPath2);
print(MinSneakyPath, MaxSneakyPath, map(evalf, AvgSneakyPath));
print(EdgeCost, PathTraffic, EdgeTraffic, TotalFlow);


// (16)
Digits = 4;
map(evalf, AvgSneakyPath);
