# kubernetes Docker and .Net(core) demo

This demo have two applications. There is an **UI** app which reach REST controller in the **Core** app, so far they has just one service that returns an array of strings. **Core** it's a statefull app thats why has a thread sleep in the Startup process to simulate the application is recovering state. 
These applications will be deployed using dokcer to generate the image and kubernetes to release it. **UI** will handle the users request that come over "internet" meanwhile **Core** will be an internal server who provide data only to the UI.


## Kubernetes dashboard
First install Kubernetes dashboard to see the server installation easier

```
#https://kubernetes.io/docs/tasks/access-application-cluster/web-ui-dashboard/
kubectl apply -f https://raw.githubusercontent.com/kubernetes/dashboard/v2.0.0/aio/deploy/recommended.yaml
kubectl proxy

# Allow Dashboard external request 
# https://www.thegeekdiary.com/how-to-access-kubernetes-dashboard-externally/
kubectl  edit service kubernetes-dashboard -n kubernetes-dashboard

#Change type: NodePort                   ### clusterIP to NodePort
kubectl -n kube-system get services

# Get port mapping to 443 by NodePort
kubectl get services -n kubernetes-dashboard

#Execute this to get a token 
kubectl -n kube-system describe $(kubectl -n kube-system get secret -n kube-system -o name | grep namespace) | grep token:

# Navigate to https://<server ip>:<port>/#/login
```


## Get demo in to the server

The folllowing step install both applications.

1. Execute the following command in the server
```
git clone https://github.com/cguillencr/kubernetes-docker-mvc-demo.git
cd kubernetes-docker-mvc-demo/Demo/
```

2.  Create a locally docker images
```
docker build -t "cguillenmendez/core:0.0.1" .
cd ../UI/
docker build -t "cguillenmendez/ui:0.0.1" .
docker images
```

Validate "cguillenmendez/core" and  "cguillenmendez/ui" images was created.

2.1 Update the image to docker.io
```
docker push cguillenmendez/core:0.0.1
docker push cguillenmendez/ui:0.0.1
```

2.2. Test application locally using just dokcer. (Not required)

```
docker run -d -p 1000:80 --name core cguillenmendez/core:0.0.1
docker run -d -p 1001:80 --name ui cguillenmendez/ui:0.0.1
docker ps -a
```
Navigate to http://<ip>:1000/api/values and validate this output
```
["value1","value2"]
```

3. Create a K8 deployment
```
 kubectl create deployment core --image=cguillenmendez/core:0.0.1
 kubectl create deployment ui --image=cguillenmendez/ui:0.0.1
 ```

4. Create a service to expose the last pod
The UI it's exposed as **NodePort** in order to reach the server externally mean while the core app it's exposed as **ClusterIp** and only pod inside the Cluster can reach it.

```
kubectl get services
kubectl expose deployment core --port 80
kubectl expose deployment ui --type="NodePort" --port 80
kubectl get services
```

Navigate to http://<ip>:<port in lastt command>/page1 and validate this output
```
["value1","value2"]
```


