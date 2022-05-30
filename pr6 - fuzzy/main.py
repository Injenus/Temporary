from PIL import Image, ImageDraw
from numpy import *
import matplotlib.pyplot as plt

im = Image.new('RGB', (500, 600), (255,255,255))
draw = ImageDraw.Draw(im)

#класс нечеткого значения
class Term:
    def __init__(self, name, x0, w):
        self.name=name
        self.x0=x0
        self.w=w
        self.left=False
        self.right=False
    def calc(self, x):
        self.last_input=x
        self.activation=self.F(x)
        return self.activation
    def F(self, x):
        if(self.left and x<=self.x0): return 1
        if(self.right and x>=self.x0): return 1
        if(x<self.x0-self.w/2): return 0
        if(x>self.x0+self.w/2): return 0
        if(x<self.x0): return -(2*self.x0/self.w-1)+2/self.w*x
        if(x>=self.x0): return (2*self.x0/self.w+1)-2/self.w*x
        return 0
    def draw(self, xmin, xmax, scale, shift):
        N=300
        dx=(xmax-xmin)/N
        for i in range(0,N):
            x=xmin+i*dx
            draw.line((int(x-dx), shift+int(scale-scale*self.F(x-dx)),
                      int(x), shift+int(scale-scale*self.F(x))), fill=128)

scale = 200

#Входная нечеткая переменная
t0=Term("SmallDist", 0, 200)
t0.left=True
t0.draw(0, 300, scale, 0)

t1=Term("MediumDist", 100, 200)
t1.draw(0, 300, scale, 0)

t2=Term("BigDist", 200, 200)
t2.right=True
t2.draw(0, 300, scale, 0)

inps = (t0, t1, t2)

#Проверка
x=30
draw.line((int(x), int(0), int(x), int(scale)), fill=128)
y0=t0.calc(x)
y1=t1.calc(x)
y2=t2.calc(x)

print("Input = ", x)
print(y0, " ", y1, " ", y2)

#Выходная нечеткая переменная
u0=Term("SmallSpeed", 10, 50)
u0.draw(0, 300, scale, 300)
u1=Term("MediumSpeed", 30, 50)
u1.draw(0, 300, scale, 300)
u2=Term("BigSpeed", 50, 50)
u2.draw(0, 300, scale, 300)

outs = (u0, u1, u2)

#нечеткий вывод по базе правил
rules=zeros(3)
rules[0]=0
rules[1]=1
rules[2]=2

#дефаззификация
center_of_mass = 0
for i in range(3):
    center_of_mass+=inps[i].activation*outs[int(rules[i])].x0

print("Result = ", center_of_mass)

im.save("img.jpg", quality=95)


#построение зависимости выхода от входа
def Fuzzy(x):
    # Входная нечеткая переменная
    t0 = Term("SmallDist", 0, 200)
    t0.left = True
    t1 = Term("MediumDist", 100, 200)
    t2 = Term("BigDist", 200, 200)
    t2.right = True
    inps = (t0, t1, t2)
    y0 = t0.calc(x)
    y1 = t1.calc(x)
    y2 = t2.calc(x)
    # Выходная нечеткая переменная
    u0 = Term("SmallSpeed", 10, 50)
    u1 = Term("MediumSpeed", 30, 50)
    u2 = Term("BigSpeed", 50, 50)
    outs = (u0, u1, u2)
    # нечеткий вывод по базе правил
    rules = zeros(3)
    rules[0] = 0
    rules[1] = 1
    rules[2] = 2
    # дефаззификация
    center_of_mass = 0
    sum=0
    for i in range(3):
        k=inps[i].activation
        center_of_mass += k * outs[int(rules[i])].x0
        sum+=k
    return center_of_mass/sum


# create 1000 equally spaced points between -10 and 10
X = linspace(0, 300, 1000)
# calculate the y value for each element of the x vector
Y = list(map(lambda x: Fuzzy(x), X))
Y=array(Y)
fig, ax = plt.subplots()
ax.plot(X, Y)
plt.savefig('plot.png')