const loginForm = document.getElementById('loginForm');

loginForm.addEventListener('submit', async (event) => {

    event.preventDefault();

    const userId   = document.getElementById('userId').value.trim();
    const password = document.getElementById('password').value;

    if (userId === "") {
        alert("아이디를 입력해주세요.");

        return;
    }
    if (password === "") {
        alert("비밀번호를 입력해주세요.");

        return;
    }
    const requestDate = {
        userId  : userId,
        password: password
    };
    try {
        const response = await fetch(`https://localhost:7226/api/Auth/login`,
            {
                method     : "POST",
                credentials: "include",
                headers    : {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(requestDate)
            }
        );
        const result = await response.json();

        if (!response.ok) {
            alert(`CODE : ${result.retVal} "${result.retMsg}"`);

            return;
        }
        console.log("로그인 성공:", result);

        alert("로그인 되었습니다.");

        const params    = new URLSearchParams(location.search);
        const returnUrl = params.get("returnUrl");

        location.href = returnUrl || "/index.html";

    } catch (error) {
        console.error("로그인 오류:", error);
        alert("서버와 통신하는 중 오류가 발생했습니다. 잠시 후 다시 시도해주세요.");
    }
});