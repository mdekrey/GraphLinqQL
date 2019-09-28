FROM anapsix/alpine-java:8

ENV ANTLR_VERSION 4.7.1
ENV CLASSPATH .:/antlr-${ANTLR_VERSION}-complete.jar:$CLASSPATH

WORKDIR /source
ADD http://www.antlr.org/download/antlr-${ANTLR_VERSION}-complete.jar /source
RUN chmod +r /source/antlr-${ANTLR_VERSION}-complete.jar

ADD https://raw.githubusercontent.com/mdekrey/graphql-java/graphql-spec-updates/src/main/antlr/Graphql.g4 /source
ADD https://raw.githubusercontent.com/mdekrey/graphql-java/graphql-spec-updates/src/main/antlr/GraphqlCommon.g4 /source
ADD https://raw.githubusercontent.com/mdekrey/graphql-java/graphql-spec-updates/src/main/antlr/GraphqlOperation.g4 /source
ADD https://raw.githubusercontent.com/mdekrey/graphql-java/graphql-spec-updates/src/main/antlr/GraphqlSDL.g4 /source

RUN java -jar /source/antlr-${ANTLR_VERSION}-complete.jar -Dlanguage=CSharp /source/Graphql.g4 -package GraphLinqQL.Ast.Antlr -visitor

WORKDIR /temp

RUN cat /source/*.cs | sed 's/package graphql\.parser\.antlr;$/ /g' > /temp/ast.g.cs

WORKDIR /dest

CMD mv /temp/*.cs /dest
