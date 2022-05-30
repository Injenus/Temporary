import math
import requests
import numpy as np
from urllib.parse import urlencode

#сессия HTTP - чтоб избежать многократных переподключений к серверу
session = requests.Session()

def send(data):
    str = urlencode(data)
    r = session.get("http://127.0.0.1:8080/?" + str, timeout=1000)
    return r.text.split("UserResult=")[1]

def normalize(x):
    norm = np.sqrt(x.dot(x))
    x = x / norm if norm != 0 else x
    return x

def normalize_ang(ang):
    while(ang<-math.pi):ang+=2*math.pi
    while(ang>math.pi):ang-=2*math.pi
    return ang

time=0

#управляющие данные
data = {'id': 0, 'v': 1, 'w': 0}

while(True):
    info=send(data)

    #обработка ответа от сервера об объектах среды
    arr=info.split('; ')
    arr_rob=np.fromstring(arr[0], dtype=float, sep=', ')
    arr_obst=np.fromstring(arr[1], dtype=float, sep=', ')
    arr_goal=np.fromstring(arr[2], dtype=float, sep=', ')

    #вектор соединяющий препятствие и робота, направленный в сторону робота
    vro=np.asarray((arr_rob[0]-arr_obst[0], arr_rob[1]-arr_obst[1]))
    Lro=np.sqrt(vro.dot(vro)) #расстояниe до препятствия
    Fro = 100**2/Lro**2 #сила обратно пропорциональна квадрату расстояния
    vro=normalize(vro)*Fro

    #вектор соединяющий цель и робота, направленный в сторону цели
    vgr=np.asarray((arr_goal[0]-arr_rob[0], arr_goal[1]-arr_rob[1]))
    Fgr=1 #сила - константа
    vgr=normalize(vgr)*Fgr

    #целевое направление робота
    v_result = 0.5*vgr+0.5*vro
    v_result=normalize(v_result)

    #расчет управления для робота
    beta = arr_rob[2]/180*math.pi #глобальный угол робота
    gamma = math.atan2(v_result[1], v_result[0]) #целевой угол
    alpha = normalize_ang(gamma - beta)

    data['w'] = 2*alpha

    print(data['w'])

    time+=1