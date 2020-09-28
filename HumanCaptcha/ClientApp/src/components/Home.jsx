import React, { Component } from 'react';
import { Alert } from 'reactstrap';
import HumanCaptcha from './HumanCaptcha';
import Config from './settings/config';

const API = Config.apiURL;
const QUERY = "test";

export class Home extends Component {
    static displayName = Home.name;

    constructor(props) {
        super(props);

        this.state = { name: "", token: "", notbot: false, message: "" };

        this.handleSubmitForm = this.handleSubmitForm.bind(this);
        this.handleChangeName = this.handleChangeName.bind(this);
        this.handleChangeCaptcha = this.handleChangeCaptcha.bind(this);
    }

    handleSubmitForm(event) {
        event.preventDefault();
        const name = this.state.name;
        const token = this.state.token;
        if (token)
            this.submitTestForm(name, token);
    }

    handleChangeCaptcha(token) {
        if (token)
            this.setState({ token: token, notbot: true });
    }

    handleChangeName(event) {
        this.setState({ name: event.target.value });
    }

    render() {
        const disabled = this.state.notbot;
        const message = this.state.message;
        let alertVisible = false;
        if (message)
            alertVisible = true;
        return (
            <div className="d-flex flex-column">
                <div className="mb-2">
                    <HumanCaptcha size="6" onChange={this.handleChangeCaptcha} />
                </div>
                <div>
                    <form onSubmit={this.handleSubmitForm}>
                        <div className="mr-2">
                            <label>
                                Name:
                                <input type="text" value={this.state.name} onChange={this.handleChangeName} disabled={!disabled} className="form-control" />
                            </label>
                        </div>
                        <div className="mt-1">
                            <input type="submit" className="btn btn-primary" value="Register" disabled={!disabled} />
                        </div>
                    </form>
                    <div className="mt-2">
                        <Alert color="info" isOpen={alertVisible}>{message}</Alert>
                    </div>
                </div>
            </div>
        );
    }

    async submitTestForm(name, token) {
        let url = API + QUERY;
        let data = { id: name };
        var result = false;
        var message = "";
        try {
            const response = await fetch(url, {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Token': token
                },
                body: JSON.stringify(data)
            });
            const resultData = await response.json();
            if (resultData) {
                result = resultData.result;
                message = resultData.message;
            }
        }
        catch (error) {
            console.log(error);
        }
        if (result)
            this.setState({ message: message });
    }
}
