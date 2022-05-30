import math
import requests
import numpy as np
from urllib.parse import urlencode
import time



#сессия HTTP - чтоб избежать многократных переподключений к серверу
session = requests.Session()

#отылка данных роботу
def send(data):
    str = urlencode(data)
    r = session.get("http://192.168.245.199:8080/?" + str, timeout=1000)
    return r.text.split("UserResult=")[1]

#нормализация вектора
def normalize(x):
    norm = np.sqrt(x.dot(x))
    x = x / norm if norm != 0 else x
    return x

#нормализация
def normalize_ang(ang):
    while(ang<-math.pi):ang+=2*math.pi
    while(ang>math.pi):ang-=2*math.pi
    return ang

T=0

USE_NET=True

if USE_NET:
    import torch
    # класс нейронной сети
    class Network(torch.nn.Module):
        def __init__(self, hidden_neurons):
            super(Network, self).__init__()
            # 1 слой принимающий сигнал
            self.fc1 = torch.nn.Linear(7, hidden_neurons)
            # 2 функция сигмоиды для второго слоя
            self.act1 = torch.nn.Sigmoid()
            # 3 функция активации для выходного слоя
            self.fc2 = torch.nn.Linear(hidden_neurons, 1)
        def forward(self, x):
            x = self.fc1(x)
            x = self.act1(x)
            x = self.fc2(x)
            return x
    # загрузка сети
    net = torch.load("our_network.torch")
    net.eval()


#управляющие данные
data = {'id': 6, 'v': 1, 'w': 0}

# f=open("dataset.txt", "w")

#цикл системы управления
while(True):
    info=send(data)

    # обработка ответа от сервера об объектах среды
    arr = info.split('; ')
    arr_rob = np.fromstring(arr[0], dtype=float, sep=', ')
    arr_obst = np.fromstring(arr[1], dtype=float, sep=', ')
    arr_goal = np.fromstring(arr[2], dtype=float, sep=', ')

    if USE_NET:
        # должно быть близко к значению из выборки
        w_ang = net.forward(torch.from_numpy(np.array(
            [arr_rob[0], arr_rob[1], arr_rob[2],
             arr_obst[0], arr_obst[1],
             arr_goal[0], arr_goal[1]]).astype(np.float32))).item() * 3
        print("Result = %f" % w_ang)
    else:
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

        w_ang = alpha

    data['w'] = w_ang

    print(data['w'])

    T+=1
    time.sleep(0.1)


#запись обучающих примеров
#file = open("dataset.txt", "w+")
#arr = [arr_rob[0], arr_rob[1], arr_goal[0], arr_goal[1], arr_obst[0], arr_obst[1], data['w']]
#file.write(str(arr));