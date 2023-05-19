# $1=host url to add to

for i in $(seq 1 70)
do
  for j in $(seq 1 70)
  do
    curl -X POST "$1?studentId=$i&courseId=$j"
  done
done
