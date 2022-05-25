# [접속하기](http://mypofol.shop)
- 컨텐츠 다운로드에 시간이 걸리고, 응답시간이 너무 길어서 접속 할 수 없다는 메시지가 뜰 수 있으나 잠시만 기다려 주세요.
- Exception alert 발생시, 캐시를 지워주세요.

# UnityOnlineProject

AWS에서 WebGL로 배포되는 Client 작성 프로젝트

**[서버 프로젝트 참조](https://github.com/FGPRJS/UnityOnlineProjectServer)**


## 프로젝트 개요

**프로젝트 배경**
- Server와 연동되는 Unity 프로젝트
- TCP/IP 소켓을 직접 사용하여 Server와 메시지를 나눌 수 있는 Client를 작성해보기
- Server와의 동기화로 다른 Client의 행동을 자신 Client에서 확인할 수 있게 하기
- 정적 웹을 기반으로 하는 Web 빌드(WebGL 배포)를 하여, Web에서 사용할 수 있게 하기

**프로젝트를 통해 얻고자 하는 것**
- 웹으로 빌드하는 것에 대해 알아보기
- 기존에 사용하던 방식으로 Server와의 통신 데이터를 읽고, 써보기
- Server와의 통신으로 상대방 Client의 제어 상황을 실시간으로 확인해 볼 수 있게 하기

## 사용 기술
- Unity (v. 21.3.2f1)
- .NET TcpClient (TCP Socket)
- JS WebSocket ([RFC 6455 Spec](https://datatracker.ietf.org/doc/html/rfc6455))

## 주요 기능

- .jslib을 통한 JS WebSocket 작성
(JS에서는 .NET Socket기능을 사용할 수 없음)

- 행동 동기화 및 다른 Client행동 동기화

![Sync](https://user-images.githubusercontent.com/92636080/170175579-9320634f-6bf9-4c70-a083-94905324dafb.gif)

- 자기자신과의 동기화 중 점프 현상을 해결하기 위한 Bumper 객체 생성

![Bumper](https://user-images.githubusercontent.com/92636080/170177300-275dbb17-9b9b-4adf-b506-855ab810a898.gif)

- 간단한 채팅

![Chat](https://user-images.githubusercontent.com/92636080/170175922-cb4b7c2b-e5e3-4175-85ff-616a94b6d6a4.gif)

