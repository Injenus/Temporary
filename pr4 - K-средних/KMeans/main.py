from PIL import Image, ImageDraw
from numpy import *


def pt(x, y, ind):
    colors=('black', 'red', 'green', 'blue', 'violet')
    r=3
    draw.ellipse((x-r, y-r, x+r, y+r), fill=colors[ind+1])
def pt2(x, y, ind):
    colors=('black', 'red', 'green', 'blue', 'violet')
    r=3
    draw.rectangle((x-r, y-r, x+r, y+r), fill=colors[ind+1])

pts=[
    [100,70, 3],
    [120,80,-1],
    [90,110,-1],
    [110,130,-1],
    [130,90,-1],
    [80,140,-1],
    [90,120,-1],
    [150,125,-1],
    [145,45,-1],
    [160,130,-1],
    [120,100,-1]
]

centers=[
    [90,80, 0],
    [150,80, 1],
    [60,140, 2]
]

def dist(p1, p2):
    dx=p1[0]-p2[0]
    dy=p1[1]-p2[1]
    return sqrt(dx*dx+dy*dy)

def findNearestCenter(p):
    minDist=100500
    bestInd=0
    for ind in range(0, len(centers)):
        c = centers[ind]
        d = dist(p, c)
        if(d<minDist):
            minDist=d
            bestInd=ind
    return bestInd

def checkAllPoints():
    for ind in range(0, len(pts)):
        # меняем цвет точки на цвет ближайшего кластера
        pts[ind][2]=findNearestCenter(pts[ind])

def findCenterOfMass(ind):
    res=[0.0,0.0]
    cnt=0
    for i in range(0, len(pts)):
        p=pts[i]
        if(p[2]==ind):
            res[0]+=p[0]
            res[1]+=p[1]
            cnt+=1
    return [res[0]/cnt, res[1]/cnt]

def shiftCenters():
    global centers
    for ind in range(0, len(centers)):
        c=centers[ind]
        c_=findCenterOfMass(ind)
        centers[ind] = [c_[0], c_[1], c[2]]

im = Image.new('RGB', (500, 300), (255, 255, 255))
draw = ImageDraw.Draw(im)


def drawAll():
    draw.rectangle((0, 0, 500, 300), fill='white') #clear

    for ind in range(0, len(pts)):
        p = pts[ind]
        pt(p[0], p[1], p[2])

    for ind in range(0, len(centers)):
        p = centers[ind]
        pt2(p[0], p[1], p[2])


#1
checkAllPoints()
drawAll()
im.save("img.jpg", quality=95)
#2
shiftCenters()
checkAllPoints()
drawAll()
im.save("img2.jpg", quality=95)
#3
shiftCenters()
checkAllPoints()
drawAll()
im.save("img3.jpg", quality=95)

def main():
    c1=findCenterOfMass(0)
    c2=findCenterOfMass(1)
    c3=findCenterOfMass(2)

    print(c1)
    print(c2)
    print(c3)

    print('Dist = {0}'.format(dist(pts[0], pts[1])))  # Press Ctrl+F8 to toggle the breakpoint.
    ind0=findNearestCenter(pts[0])
    print('Ind(0) = {0}'.format(ind0))
    
   
main()