document.addEventListener("DOMContentLoaded", async () => {

    try {
        const response = await fetch("https://localhost:7226/api/Auth/me",
            {
                credentials: "include"
            });
        if (response.ok) {

            alert("이미 로그인 상태입니다.");

            location.href = "/index";

            return
        }

        if (response.status === 401) {

            console.log("회원가입 가능");
        }
    } catch (error) {

        console.error("로그인 상태 확인 실패 : ", error);
    }
})
function addr_search() {
    new daum.Postcode({
        oncomplete: function(data) {

            console.log(data);

            var roadAddr  = data.roadAddress;  //도로명 주소 변수
            var jibunAddr = data.jibunAddress; //지번 주소 변수

            document.getElementById('zipcode').value = data.zonecode;

            if (roadAddr !== '') {
                document.getElementById('base_addr').value = roadAddr;

            } else if (jibunAddr !== '') {
                document.getElementById('base_addr').value = jibunAddr;
            }
        }
    }).open();
}

document.addEventListener("DOMContentLoaded", () => {

    const password      = document.getElementById("password");
    const passwordCheck = document.getElementById("passwordCheck");

    const pwdOk = document.getElementById("pwd_ok");
    const pwdNo = document.getElementById("pwd_no");

    passwordCheck.addEventListener("blur", () => {

        const passwordValue      = password.value;
        const passwordCheckValue = passwordCheck.value;

        // 비밀번호 길이 확인
        if (passwordValue.length < 9 || passwordValue.length > 18) {

            alert("비밀번호는 9자리 이상 18자리 이하로 입력하세요.");

            password.value      = "";
            passwordCheck.value = "";

            pwdOk.classList.add("d-none");
            pwdNo.classList.add("d-none");

            return;
        }

        // 비밀번호 확인
        if (passwordValue === passwordCheckValue) {

            pwdOk.classList.remove("d-none");
            pwdNo.classList.add("d-none");

        } else {

            passwordCheck.value = "";

            pwdOk.classList.add("d-none");
            pwdNo.classList.remove("d-none");
        }
    });
});

const userId         = document.getElementById("userId");
const checkUserIdBtn = document.getElementById("checkUserIdBtn");

const idOk      = document.getElementById("id_ok");
const idAlready = document.getElementById("id_already");

checkUserIdBtn.addEventListener("click", async () => {

    const value = userId.value.trim();

    if (value === "") {
        alert("아이디를 입력하세요.");
        userId.focus();
        return;
    }

    try {

        const url = `https://localhost:7226/api/Auth/check-userid?userId=${encodeURIComponent(value)}`;

        const response = await fetch(url);

        const text = await response.text();

        console.log("응답 내용:", text);

        if (!response.ok) {
            throw new Error(`HTTP ${response.status}`);
        }

        const result = JSON.parse(text);

        if (result.available) {

            idOk.classList.remove("d-none");
            idAlready.classList.add("d-none");

        } else {

            idOk.classList.add("d-none");
            idAlready.classList.remove("d-none");
        }

    } catch (error) {

        console.error("아이디 중복 확인 오류:", error);

        alert("아이디 중복 확인에 실패했습니다.");
    }
});

registerForm.addEventListener("submit", async function (event) {

    event.preventDefault(); // form 기본 제출 방지

    const userId = document.getElementById("userId").value.trim();
    const password = document.getElementById("password").value.trim();
    const userName = document.getElementById("userName").value.trim();
    const email = document.getElementById("email").value.trim();
    const gender = document.getElementById("gender").value;

    const birthDate = document.getElementById("birth_date").value;
    const phoneNumber = document.getElementById("telnum").value.trim();
    const zipCode = document.getElementById("zipcode").value.trim();
    const baseAddress = document.getElementById("base_addr").value.trim();
    const detailAddress = document.getElementById("dtl_addr").value.trim();

    // 필수값 확인
    if (userId === "") {
        alert("아이디를 입력하세요.");
        return;
    }

    if (password === "") {
        alert("비밀번호를 입력하세요.");
        return;
    }

    if (userName === "") {
        alert("이름을 입력하세요.");
        return;
    }

    if (email === "") {
        alert("이메일을 입력하세요.");
        return;
    }

    if (gender === "") {
        alert("성별을 선택하세요.");
        return;
    }

    if (birthDate === "") {
        alert("생년월일을 입력하세요.");
        return;
    }

    if (phoneNumber === "") {
        alert("전화번호를 입력하세요.");
        return;
    }

    if (zipCode === "" || baseAddress === "" || detailAddress === "") {
        alert("주소를 모두 입력하세요.");
        return;
    }

    const requestDate = {
        userId       : userId,
        passwordHash : password,
        userName     : userName,
        email        : email,
        phoneNumber: phoneNumber,

        gender       : gender,
        birthDate    : birthDate,
        zipCode      : zipCode,
        baseAddress  : baseAddress,
        detailAddress: detailAddress
    };

    try {
        const response = await fetch("https://localhost:7226/api/Auth/register", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(requestDate)
        });
        const result = await response.json();

        if (result.retVal !== 0) {
            alert(result.message || "회원가입에 실패했습니다.");

            return;
        }
        alert(result.retMsg || "회원가입이 완료되었습니다.");

        location.href = "/accounts/login.html";

    } catch (error) {
        console.error("회원가입 오류:", error);

        alert("회원가입 중 오류가 발생했습니다.");
    }
});