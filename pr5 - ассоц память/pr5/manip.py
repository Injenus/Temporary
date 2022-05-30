import math
import sys, pygame
from pygame.locals import*
import numpy as np

width=1000
height=500
Color_screen=(255,255,255)
Color_line=(255,0,0)
Color_pos=(0,255,0)

r=10
timer = pygame.time.Clock()
fps=30

p0 = (500, 200) #точка основания
L1 = 150 #длина 1го звена
L2 = 100 #длина 2го звена
a1 = 0.5
a2 = 0.3

manual_mode=False

table=[]
AM=np.array(table)

#сравнение вектора входного и эталонного
def error(vec1, vec2):
    return (((vec1-vec2)**2).mean())**0.5
    end

def main():
    screen=pygame.display.set_mode((width,height))

    global x, y, a1, a2, p0, table, pos, manual_mode, AM
    p0=np.array(p0, dtype=float)
    pos=np.array(p0, dtype=float)

    while True:
        for events in pygame.event.get():
            if events.type == QUIT:
                print(table)
                # with(open('table.txt', 'w')) as f:
                #     np.savetxt('table.txt', table, delimiter=',', fmt='%f')
                sys.exit(0)
            # handle MOUSEBUTTONUP
            if events.type == pygame.MOUSEBUTTONUP:
                pos = pygame.mouse.get_pos()
            if events.type == pygame.MOUSEMOTION:
                if(manual_mode):
                    pos = pygame.mouse.get_pos()

        screen.fill(Color_screen)

        pygame.draw.ellipse(screen, Color_line,
                            np.append(p0-np.array([r, r]), [2*r, 2*r]))

        pygame.draw.ellipse(screen, Color_pos,
                            np.append(pos - np.array([r, r]), [2 * r, 2 * r]))

        # конец первого звена
        p1=p0+np.array((L1*math.cos(a1), L1*math.sin(a1)))
        pygame.draw.line(screen, Color_line, p0, p1, 5)
        # конец второго звена
        p2=p1+np.array((L2*math.cos(a1+a2), L2*math.sin(a1+a2)))
        pygame.draw.line(screen, Color_line, p1, p2, 5)

        if(manual_mode):
            #использование ассоциативной памяти (таблицы)
            ibest=0
            emin=100500
            for i in range(0, len(AM)):
                A=AM[i][2:4] # берем хранимые значения x, y
                B=np.array(pos) # берем x, y от клика мышки
                e=error(A, B) # сравниваем текущую позицию с эталоном
                if(e<emin):
                    emin=e
                    ibest=i

            best=AM[ibest] # наилучшая запись ассоциативной памяти
            a1=best[0]
            a2=best[1]
        else:
            table.append((a1, a2, p2[0], p2[1]))
            a1+=0.1
            a2-=0.0389236987

        pygame.display.flip()
        timer.tick(fps)

        if(len(table)>1000):
            if(not manual_mode): print(table)
            manual_mode=True
            AM = np.array(table)


main()


