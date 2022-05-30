import numpy as np
import ast
import math
import torch
import numpy as np
import matplotlib.pyplot as plt

#функция прореживания обучающей выборки
def skip_elements(elements, n):
    new_list = [ ]
    i = 0
    for element in elements:
        if i%n==0:
         c=elements[i]
         new_list.append(c)
        i+=1
    return np.array(new_list)

#функция перемешивания обучающих данных
def shuffle_along_axis(a, axis):
    idx = np.random.rand(*a.shape).argsort(axis=axis)
    return np.take_along_axis(a,idx,axis=axis)

#загрузка текстового файла
lines = open('lines6.txt','r')

data = np.array([ast.literal_eval(line.strip(',\n')) for line in lines])
data = data.astype(np.float32)
print(str(len(data)) + " lines")

data = skip_elements(data, 5)
data = shuffle_along_axis(data, 0)
print(str(len(data)) + " lines")

cols_inp=[0, 1, 2, 5, 6, 8, 9]
cols_out=[3]

N=2000 #число обучающих примеров
Nt=400 #число тестовых примеров

#преобразование массива, получение входных и выходных данных
x_train=torch.from_numpy(data[:N, cols_inp])
y_train=torch.from_numpy(data[:N, cols_out]).unsqueeze(1)
x_test=torch.from_numpy(data[N:N+Nt, cols_inp])
y_test=torch.from_numpy(data[N:N+Nt, cols_out]).unsqueeze(1)

#класс нейронной сети
class Network(torch.nn.Module):
    def __init__(self, hidden_neurons):
        super(Network, self).__init__()
        # 1 слой принимающий сигнал
        self.fc1=torch.nn.Linear(7, hidden_neurons)
        # 2 функция сигмоиды для второго слоя
        self.act1=torch.nn.Tanh()
        # 3 функция активации для выходного слоя
        self.fc2=torch.nn.Linear(hidden_neurons, 1)

    def forward(self, x):
        x = self.fc1(x)
        x = self.act1(x)
        x = self.fc2(x)
        return x

#экземпляр нейронной сети
our_network = Network(20)

#функция потерь
def loss(pred, target):
    ans=(pred-target)**2
    return ans.mean()

#способ оптимизации
optimizer = torch.optim.Adam(our_network.parameters(), 0.01)

#обучение
for i in range(200):
    optimizer.zero_grad()
    y_pred = our_network.forward(x_train)
    loss_val = loss(y_pred, y_train)
    loss_val.backward()
    optimizer.step()
    if(i%5==0): #в начале каждой эпохи
        y_pred = our_network.forward(x_test)
        print((((y_pred-y_test)**2).mean())**0.5)
        print("Loss = %f"%loss_val)
        #predict(our_network, x_test, y_test)
        ##plt.savefig("mygraph-%d.png"%i)

#сохранение сети
torch.save(our_network, "our_network.torch")

#загрузка сети (в качестве примера)
net = torch.load("our_network.torch")
net.eval()

# должно быть близко к значению из выборки (в качестве примера)
y=net.forward(torch.from_numpy(np.array([104.531555, 522.11304, 25, 314, 380, 857, 350]).astype(np.float32)))
print("Result = %f"%y)