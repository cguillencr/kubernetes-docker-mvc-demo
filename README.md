# kubernetes Docker and .Net(core) demo


Demo it's a core project which returns a json response in a basic REST GET controller. This application will be deployed using dokcer to generate the image and kubernetes to release it.


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
1. Execute the following command in the server
```
git clone https://github.com/cguillencr/kubernetes-docker-mvc-demo.git
cd kubernetes-docker-mvc-demo/Demo/
```

2.  Create a locally docker image
```
docker build -t "cguillenmendez/core:api" .
docker images
```

Validate image "cguillenmendez/core" was created.

2.1 Update the image to docker.io
```
docker push cguillenmendez/core:api
```


3. Test application locally using just dokcer 

```
docker run -d -p 8080:80 --name demo cguillenmendez/core:api
docker ps -a
```
Navigate to http://<ip>:8080/api/values and validate this output
```
["value1","value2"]
```

4. Create a K8 deployment
```
 kubectl create deployment k8demo --image=cguillenmendez/core:api
 ```

5. Create a service to expose the last pod
```
kubectl get services
kubectl expose deployment k8demo --type="NodePort" --port 80
kubectl get services
```

Navigate to http://<ip>:<port in lastt command>/api/values and validate this output
```
["value1","value2"]
```


