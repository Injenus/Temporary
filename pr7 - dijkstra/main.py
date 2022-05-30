import numpy as np
from PIL import Image
import sys

class Edge:
    def __init__(self, N1, N2):
        self.N1 = N1
        self.N2 = N2
        self.w = 0 #сложность перехода из узла в узел
class Node:
    def __init__(self, X, Y):
        self.Edges=[]
        self.X = X
        self.Y = Y
        self.visited = False #сложность перехода из узла в узел
    def SetWeight(self, N2, w):
        for i in range(len(self.Edges)):
            if(self.Edges[i].N2==N2):
                self.Edges[i].w = w
                break
    def GetWeight(self, N2):
        for i in range(len(self.Edges)):
            if(self.Edges[i].N2==N2):
                return self.Edges[i].w
        return sys.float_info.max
    def AddEdge(self, N2):
        dx=self.X-N2.X
        dy=self.Y-N2.Y
        e=Edge(self, N2)
        e.w=np.sqrt(dx*dx + dy*dy)
        self.Edges.append(e)
class Graph:
    def __init__(self, w, h):
        self.Nodes = [[Node(x,y) for x in range(w)] for y in range(h)]
        self.w = w
        self.h = h
        self.ConnectNodes()
    def ConnectNodes(self):
        for i in range(self.w):
            for j in range(self.h):
                for k in range(i-1, i+2):
                    for n in range(j-1, j+2):
                        if(k<0 or n<0 or k>=self.w or n>=self.h):
                            continue
                        if(k==i and n==j):
                            continue
                        self.Nodes[j][i].AddEdge(self.Nodes[n][k])
    def AddObstacle(self, i, j, P):
        for k in range(i-1, i+2):
            for n in range(j-1, j+2):
                if(k<0 or n<0 or k>=self.w or n>=self.h):
                    continue
                if(k==i and n==j):
                    continue
                weight = max(P, self.Nodes[n][k].GetWeight(self.Nodes[j][i]))
                self.Nodes[n][k].SetWeight(self.Nodes[j][i], weight)
    def AddObstacles(self, image):
        for i in range(image.shape[1]):  # x
            for j in range(arr.shape[0]):  # y
                px = image[j, i]
                br = sum(px)
                p = 1 - br / 255 / 3
                if (p > 0):
                    self.AddObstacle(i, j, 100000 * p)
    def FindWay(self, Start, End):
        Dist_old = {}
        Dist_new = {}
        Dist_old[Start] = Dist_new[Start] = 0
        prev = {}
        for i in range(self.w):
            for j in range(self.h):
                n = self.Nodes[j][i]
                n.visited = False
                Dist_old[n] = sys.float_info.max
                if (n == Start): Dist_old[n] = 0
                Dist_new[n] = 0

        front = [Start]
        while (len(front) > 0):
            curr = self.FindBestFrontNode(front, Dist_old) # !!! сортировать + можно добавить эвристику(A *)
            curr.visited = True
            front.remove(curr)

            for i in range(len(curr.Edges)):
                next = curr.Edges[i].N2
                if (next.visited):
                    continue

                if (next not in front):
                    front.append(next)

                Dist_new[next] = Dist_old[curr] + curr.GetWeight(next)
                if (Dist_new[next] < Dist_old[next]):
                    Dist_old[next] = Dist_new[next]
                    prev[next] = curr

        Res = []
        if (End not in prev.keys()):
            return None

        curr = prev[End]
        Res.append(End)
        while (True):
            if (curr == Start):
                break
            Res.append(curr)

            if (End not in prev.keys()):
                return None
            curr = prev[curr]

        Res.append(Start)
        Res.reverse()

        return Res

    def FindBestFrontNode(self, front, dist_old):
        res = None
        min = sys.float_info.max
        for i in range(len(front)):
            n = front[i]
            d = dist_old[n]
            if (d < min):
                min = d
                res = n
        return res

img = Image.open("map.png") # filename is the png file in question
arr = np.array(img)

graph=Graph(arr.shape[1], arr.shape[0])
graph.AddObstacles(arr)

path = graph.FindWay(graph.Nodes[0][0], graph.Nodes[arr.shape[0] - 1][arr.shape[1] - 1])
for i in range(len(path)):
    n = path[i]
    arr[n.Y, n.X]=[255, 0, 0]

im = Image.fromarray(arr)
im.save("result.png")
